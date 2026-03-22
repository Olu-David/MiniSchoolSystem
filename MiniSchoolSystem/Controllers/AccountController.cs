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

            // 1. Validation & Password Null Check
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }
            var user = new UserDb { Email = model.Email };
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var ConfirmationLink = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token }, Request.Scheme);

            var result = await _userService.RegistrationAsync(model, ConfirmationLink);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description.ToString());
                    return View(model);
                }
            }
            Temp["Error"]="Registration Successful";

            return RedirectToAction(nameof(Login));


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
            var result = await _userService.ConfirmMailAsync(user, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess"); // A view that says "Success!"
            }

            return View("Error");
        }
        [HttpPost]
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
                await _userService.Send2FAAsync(user);
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Home");
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
            var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
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
