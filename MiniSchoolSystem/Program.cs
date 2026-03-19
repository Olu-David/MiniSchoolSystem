using Microsoft.AspNetCore.Authentication.Cookies;
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

            // ── 1. Services ───────────────────────────────────────
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IUserService, UserService>(); // Keep only one
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();

            // ── 2. Cookie Policy ──────────────────────────────────
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ── 3. Database ───────────────────────────────────────
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
                var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                             ?? builder.Configuration.GetConnectionString("DefaultConnection");

                var npgsqlConn = ConvertPostgresUrl(rawUrl!);

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(npgsqlConn));
            }

            // ── 4. Identity & Auth ────────────────────────────────
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
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie.Name = "MiniSchoolCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSetting"));


            // ── 5. Middleware Pipeline ───────────────────────────
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection(); // Only redirect locally
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                // DO NOT use HttpsRedirection here; let Render handle it.
            }
            //6/////////////DATA PROTECTION
            builder.Services.AddDataProtection()
             .PersistKeysToDbContext<AppDbContext>();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            // ── 6. Auto-migrate ──────────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Force migration on startup to ensure tables exist
                    context.Database.Migrate();
                    Console.WriteLine("SabiSpace: Migration Successful!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "SabiSpace Error: Migration failed on startup.");
                }
            }

         

            app.Run();
        }

        private static string ConvertPostgresUrl(string url)
        {
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                   $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}