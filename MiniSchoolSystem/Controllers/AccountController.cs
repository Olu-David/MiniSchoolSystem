using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ILogger<AccountController> _logger;
        private readonly SignInManager<UserDb> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger, SignInManager<UserDb> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AcceptCokkies()
        {
            Response.Cookies.Append("CookieAccepted", "true",
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddYears(1)
                });
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 1. Validation & Password Null Check
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }

            // 2. Map ViewModel to UserDb
            var user = new UserDb
            {
                UserName = model.UserName,
                Email = model.Email,
                // Logic: Store section only if user is a Parent
                UserSection = model.Role == "Parent" ? model.Section : null,
                FullName = model.FirstName + " " + model.LastName,
                TwoFactorEnabled = true, // Your security requirement
                EmailConfirmed = false   // Must be false until they click the link
            };

            // 3. Create the User in the Database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // --- 4. EMAIL CONFIRMATION TOKEN ---
            // This generates a unique security string for this specific user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create the full URL link
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            // PRACTICE TIP: Check your "Output" window in Visual Studio to see this link!
            System.Diagnostics.Debug.WriteLine($"Confirmation Link: {confirmationLink}");

            // --- 5. ROLE MANAGEMENT ---
            string[] roles = { "Student", "Parent" };
            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    // Fixed the parenthesis syntax error here
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 6. Assign the Role to the User
            if (!string.IsNullOrEmpty(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // 7. Success Redirect
            // Redirect to a page telling them to check their email
            return View("RegisterSuccessInfo");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            // Verify the token and flip the EmailConfirmed bit in the DB
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess"); // A view that says "Success!"
            }

            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password!,
                model.RememberMe,
                false);

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("Verify2FA");
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError("", "Invalid login");

            return View(model);
        }
    }
}