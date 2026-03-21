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

            // ── 1. MVC ────────────────────────────────────────────
            builder.Services.AddControllersWithViews();

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
        options.UseNpgsql(localConn)  // ✅ Changed from UseSqlServer to UseNpgsql
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

                // You can turn this to true later after email works
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── 5. Cookie Auth ────────────────────────────────────
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Home/Index";
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

            // ── 8. User Service ───────────────────────────────────  
            
             builder.Services.AddScoped<IUserService, UserService>();

            
            // ── 9. Course Service ───────────────────────────────────  
            
             builder.Services.AddScoped<ICourseService, CourseService>();
            

            // This tells ASP.NET: "Save my security keys in the database table we created"
            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<AppDbContext>();

            // ── BUILD APP ─────────────────────────────────────────
            var app = builder.Build();

            // ── ✅ FIXED: ERROR HANDLING (VERY IMPORTANT) ─────────
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // 🔥 SHOW REAL ERRORS
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // ── Middleware ───────────────────────────────────────
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            // ── Routes ───────────────────────────────────────────
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }

        // ── PostgreSQL Converter ────────────────────────────────
        private static string ConvertPostgresUrl(string url)
        {
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');

            return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                   $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}
