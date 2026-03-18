using Microsoft.AspNetCore.Authentication.Cookies;
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

            // ── 1. MVC ────────────────────────────────────────────
            builder.Services.AddControllersWithViews();

            // ── 2. Cookie Policy ──────────────────────────────────
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ── 3. Database ───────────────────────────────────────
            // FIX: Only register DbContext ONCE, resolving connection string properly
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
                // FIX: Render gives DATABASE_URL in postgres://user:pass@host/db format
                // Npgsql needs host=...;database=... format — convert it here
                var rawUrl = Environment.GetEnvironmentVariable("postgresql://sabispacedb_user:Mi5f6JIoFv44SO5tHAqAGLnkKVBJ06ar@dpg-d6t912hj16oc73f6c740-a/sabispacedb")
                             ?? builder.Configuration.GetConnectionString("DefaultConnection");

                var npgsqlConn = ConvertPostgresUrl(rawUrl!);

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(npgsqlConn));   // FIX: pass converted string
            }

            // ── 4. Identity ───────────────────────────────────────
            builder.Services.AddIdentity<UserDb, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;

                // FIX: Set to false unless your EmailService is confirmed working on Render.
                // Flip back to true only after testing email confirmation end-to-end.
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── 5. Cookie Auth ────────────────────────────────────
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie.Name = "MiniSchoolCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // ── 6. Email Service ──────────────────────────────────
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSetting"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            // ── 7. File Service ───────────────────────────────────
            builder.Services.AddScoped<IFileService, FileService>();

            // FIX: Removed the fragile ILogger singleton — ASP.NET Core's built-in
            // logging already provides ILogger<T>. Inject ILogger<LessonController>
            // directly in your controller constructor instead.

            // ─────────────────────────────────────────────────────
            var app = builder.Build();
            // ─────────────────────────────────────────────────────

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

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