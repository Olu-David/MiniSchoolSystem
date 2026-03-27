using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using Npgsql.Internal;
using Org.BouncyCastle.Utilities;
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            
            return RedirectToAction("Index", "Home");
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
                _ = Task.Run(() => _emailService.SendEmailAsync(user.Email ?? "null", "Confirm your SabiSpace Account",
                    $"Please confirm your account by clicking here: <a href='{confirmationLink}'>Click Here</a>")) ;

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
        public IActionResult EmailMessage(string email)
        {
          ViewBag.Email = email;
          ViewBag.Message = "Your account is not confirmed. We’ve sent you a new confirmation link.";
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
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid or token Expired";
                return RedirectToAction("Login"); // Or your ResendConfirmation action
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("User Not Found");
            }

            try
            {
                // 🚩 CRITICAL STEP: Decode the token from Base64 back to its original format
                var decodedTokenBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(token);
                var originalToken = System.Text.Encoding.UTF8.GetString(decodedTokenBytes);

                // Pass the ORIGINAL token to your service
                var result = await _userService.ConfirmMailAsync(user, originalToken);

                if (result.Succeeded)
                {
                    return View("ConfirmEmailSuccess");
                }
            }
            catch (Exception)
            {
                // If decoding fails, the token was tampered with or malformed
                return View("Error");
            }

            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            // 1.Safety Check: Always check if email is provided
                 if (string.IsNullOrEmpty(email)) return BadRequest("Email is required.");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return View("EmailMessage");

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
            _ = Task.Run(() => _emailService.SendEmailAsync(user.Email ?? "null", "Confirm your SabiSpace Account",
                $"Please confirm your account by clicking here: <a href='{confirmationLink}'>Click Here</a>"));

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


        [HttpGet]
        public IActionResult SendTwoFactorAuthentication()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTwoFactorAuthentication(string Id)
        {
            var appUser = await _userManager.FindByIdAsync(Id);
            if(appUser==null||!appUser.EmailConfirmed)
            {
                ModelState.AddModelError("", "User Not Found");
                return RedirectToAction(nameof(Registration));
            }
            var result = _userService.Send2FAAsync(appUser);
            
            if(result==null)
            {
                return RedirectToAction(nameof(Login));
            }
            return RedirectToAction(nameof(Verify2FaAuth));
        }
        [HttpGet]
        public IActionResult Verify2FaAuth()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify2FaAuth(Verify2FAViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Verify the code
            var result = await _userService.Verify2FAAsync(model);

            if (result.Succeeded)
            {
                // 2. Now that they are officially IN, find who they are
                var appUser = await _userManager.FindByEmailAsync(model.Email);
                if(appUser==null) return RedirectToAction(nameof(Login));
            
                if (await _userManager.IsInRoleAsync(appUser, "Student"))
                    return RedirectToAction("Index", "Student");

              
                
            }
            ModelState.AddModelError("", "Invalid Verification Code"); 
            return RedirectToAction("Index", "Home");
        
            
        }



        
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewDTO model)
{
    if (!ModelState.IsValid) return View(model);

    var appUser = await _userManager.FindByEmailAsync(model.Email ?? "");

    if (appUser == null)
    {
        ModelState.AddModelError("", "Invalid login attempt");
        return View(model);
    }

    var (result, requires2FA) = await _userService.LoginUserAsync(model);

    // 1️⃣ Handle 2FA
    if (requires2FA)
    {
        TempData["2FAEmail"] = model.Email;
        return RedirectToAction("TwoFactor", "Account");
    }

    // 2️⃣ Handle email not confirmed
    if (result.IsNotAllowed && !appUser.EmailConfirmed)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        var encodedToken = System.Text.Encoding.UTF8.GetBytes(token);
        encodedToken = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(encodedToken);

        var confirmationLink = Url.Action(
            "ConfirmEmail",
            "Account",
            new { userId = appUser.Id, token = encodedToken },
            Request.Scheme);

        await _emailService.SendEmailAsync(
            appUser.Email!,
            "Confirm your account",
            $"Click here to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>"
        );

        return RedirectToAction("EmailMessage", new { email = appUser.Email });
    }

    // 3️⃣ Invalid login
    if (!result.Succeeded)
    {
        ModelState.AddModelError("", "Invalid login attempt");
        return View(model);
    }

    // 4️⃣ Role-based redirect
    if (await _userManager.IsInRoleAsync(appUser, "SuperAdmin"))
        return RedirectToAction("Index", "SuperAdmin");

    if (await _userManager.IsInRoleAsync(appUser, "Admin"))
        return RedirectToAction("Index", "Admin");

    if (await _userManager.IsInRoleAsync(appUser, "Teacher"))
        return RedirectToAction("Index", "Teacher");

    if (await _userManager.IsInRoleAsync(appUser, "Student"))
        return RedirectToAction("Index", "Student");

    return RedirectToAction("Index", "Home");
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
            ModelState.AddModelError("", "Invalid Password");
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
