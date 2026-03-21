using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniSchoolSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly SignInManager<UserDb> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, IUserService userService, SignInManager<UserDb> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
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
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            // 1. Basic Validation
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }

           
            var result = await _userService.RegistrationAsync(model, null);

            if (result.Succeeded)
            {
                // Step B: Now the user exists in the DB! Find them to get their ID.
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) return Unauthorized();

                // Step C: Generate the Token & Link
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                    System.Text.Encoding.UTF8.GetBytes(token));

                var confirmationLink = Url.Action(
                    action: "ConfirmEmail",
                    controller: "Auth",
                    values: new { userId = user.Id, token = encodedToken },
                    protocol: Request.Scheme);

                // Step D: Now send the email using your service
                await _emailService.SendEmailAsync(user.Email??"null", "Confirm your SabiSpace Account",
                    $"Please confirm your account by clicking here: <a href='{confirmationLink}'>Click Here</a>");

                // 3. Show the "Check your inbox" page
                ViewBag.Success = false;
                ViewBag.Email = model.Email;
                return RedirectToAction(nameof(EmailMessage));


                
            }

            // 4. Handle Errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult EmailMessage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["Error"] = "Invalid or token Epired";
                return RedirectToAction(nameof(ResendConfirmation));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Success = false;

                return Unauthorized( "User Not Found");
            }


            // Verify the token and flip the EmailConfirmed bit in the DB
            var result = await _userService.ConfirmMailAsync(user, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess"); // A view that says "Success!"
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            // Step C: Generate the Token & Link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Action(
                action: "ConfirmEmail",
                controller: "Auth",
                values: new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme);

            // Step D: Now send the email using your service
            await _emailService.SendEmailAsync(user.Email ?? "null", "Confirm your SabiSpace Account",
                $"Please confirm your account by clicking here: <a href='{confirmationLink}'>Click Here</a>");

            // 3. Show the "Check your inbox" page
            ViewBag.Success = false;
            ViewBag.Email = email;
            return View("EmailMessage");
        }

        private IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("SuperAdmin")) return RedirectToAction("Index", "SuperAdmin");
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
            if (User.IsInRole("Teacher")) return RedirectToAction("Index", "Teacher");
            if (User.IsInRole("Student")) return RedirectToAction("Index", "Student");

            // No recognised role → send to Home
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login( )
        {
            return View();
        }

            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new UserDb { Email = model.Email };
            var (result, requires2FA) = await _userService.LoginUserAsync(model);


            if (requires2FA)
            {
                TempData["2FAEmail"] = model.Email;
                await _userService.Send2FAAsync(user);
            }

            if (result.Succeeded)
            {
                return RedirectToDashboard();
            }

            ModelState.AddModelError("", "Invalid login");

            return View(model);
        }
        [HttpGet]
        public IActionResult DeactivateAccount()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeactivateAccount(UserDb user)
        {
            if (user == null)
                return NotFound();

            var result = await _userService.DeactivateAccountAsync(user);

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _userService.ForgotPasswordAsync(email);

            return RedirectToAction(nameof(ResetPassoword));
        }
        [HttpGet]
        public IActionResult ResetPassoword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userService.ResetPasswordAsync(model.Email??"null", model.Token??"null", model.NewPassword ?? "null"    );
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                   
                }
                     return View(model);
              

            }
            return RedirectToAction(nameof(Login));
        }
    }
}