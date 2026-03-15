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
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"))
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(w =>
                        w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics
                            .RelationalEventId.PendingModelChangesWarning)));

            // ── 4. Identity ─────────────
            builder.Services.AddIdentity<UserDb, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── 5. Cookie Auth 
            //       ───────────────────────
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

            // ── 8. Fix ILogger injection in LessonController ──────
            builder.Services.AddSingleton(typeof(ILogger),
                typeof(Logger<Program>));

            // ─────────────────────────────────────────────────────
            var app = builder.Build(); // ← Build AFTER all services
            // ─────────────────────────────────────────────────────

            // ── Middleware Pipeline ───────────────────────────────
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication(); // ← before UseAuthorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}