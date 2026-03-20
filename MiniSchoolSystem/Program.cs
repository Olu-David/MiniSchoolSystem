using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Services;
using MiniSchoolSystem.Implementation.Settings;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── 1. Thread pool — prevents OOM crash on Render free tier (512MB)
            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(10, 10);

            // ── 2. Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();

            // ── 3. Cookie Policy
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ── 4. Database
            if (builder.Environment.IsDevelopment())
            {
                var localConn = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(localConn)
                        .EnableSensitiveDataLogging()
                        .ConfigureWarnings(w =>
                            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics
                                .RelationalEventId.PendingModelChangesWarning)));
            }
            else
            {
               
                // ConvertPostgresUrl() — a null string causes a hard crash (status 139)
                var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                             ?? builder.Configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(rawUrl))
                    throw new InvalidOperationException(
                        "DATABASE_URL environment variable is not set. " +
                        "Add it in your Render dashboard under Environment Variables.");

                var npgsqlConn = ConvertPostgresUrl(rawUrl);

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(npgsqlConn));
            }

            // ── 5. Identity & Auth
            builder.Services.AddIdentity<UserDb, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.Cookie.Name = "SabiSpaceCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSetting"));

            // ── 6. Data Protection
            // BUG FIX 2: AddDataProtection() was called AFTER builder.Build()
            // — services can only be registered BEFORE Build() is called.
            // Calling it after causes a silent crash on startup.
            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<AppDbContext>();

            // ── 7. Build the app
            var app = builder.Build();

            // ── 8. Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                // Do NOT call UseHttpsRedirection() on Render — it handles TLS itself
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            // BUG FIX 3: Default route was pointing to "Account" controller
            // but your controller is called "Auth". This caused every page to 404.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ── 9. Auto-migrate on startup (already correctly wrapped in try/catch)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                    Console.WriteLine("SabiSpace: Migration successful!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "SabiSpace: Migration failed on startup.");
                    // Do NOT rethrow — a migration failure should not crash the whole app
                }
            }

            // ── 10. Port binding — Render assigns a dynamic PORT env variable
            // BUG FIX 4: app.Run() with no argument ignores Render's $PORT
            // and the health check fails, causing Render to kill the process (status 139)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            Console.WriteLine($"SabiSpace: Starting on port {port}");
            app.Run($"http://0.0.0.0:{port}");
        }

        // ── Converts a Postgres DATABASE_URL to a Npgsql connection string
        private static string ConvertPostgresUrl(string url)
        {
            // BUG FIX: wrap in try/catch so a malformed URL doesn't hard-crash
            try
            {
                var uri = new Uri(url);
                var userInfo = uri.UserInfo.Split(':');
                return $"Host={uri.Host};" +
                       $"Port={uri.Port};" +
                       $"Database={uri.AbsolutePath.TrimStart('/')};" +
                       $"Username={userInfo[0]};" +
                       $"Password={userInfo[1]};" +
                       $"SSL Mode=Require;" +
                       $"Trust Server Certificate=true";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Could not parse DATABASE_URL '{url}'. " +
                    $"Expected format: postgres://user:password@host:port/database. " +
                    $"Inner error: {ex.Message}");
            }
        }
    }
}