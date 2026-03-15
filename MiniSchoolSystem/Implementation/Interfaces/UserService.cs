using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserDb> _signInManager;
        private readonly IEmailService _emailService;

        public UserService(UserManager<UserDb> userManager, ILogger<UserService> logger, RoleManager<IdentityRole> roleManager, SignInManager<UserDb> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

       
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {

            var user = new UserDb()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FirstName + "" + model.LastName,
                UserSection = model.Role == "Student" && model.Role != "Parent" ? model.Section : null,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                PhoneNumber = model.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                if (model.Role == "Student" && model.Role == "Parent")
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                }
            };
            //GenerateToken
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //Encoded Token
            var Encodedtoken= Uri.EscapeDataString(token);
            var confirmationLink = $"https;//Thinkly.com//\account/Confirm-Email?userId{user.Id}&token={Encodedtoken}";

            await _emailService.SendEmailAsync(user.Email, "Confirm Your ThinklyAccount", $"Click this link to confirm your account: <a href='{confirmationLink}'>ConfirmEmail </a>");
            return result;

        }
        public async Task<(SignInResult result, bool requires2FA)> LoginUserAsync(LoginViewDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            // 1. Basic checks
            if (user == null) return (SignInResult.Failed, false);
            if (!user.EmailConfirmed) return (SignInResult.NotAllowed, false);

            // 2. Perform the sign-in check (this handles password AND 2FA status)
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            // 3. Handle 2FA Requirement
            if (signInResult.RequiresTwoFactor)
            {
                await SendTwoFactorCodeAsync(user);
                return (SignInResult.TwoFactorRequired, true);
            }

            // 4. Handle Success
            if (signInResult.Succeeded)
            {
                return (SignInResult.Success, false);
            }

            // 5. Default to Failure (Incorrect password, etc.)
            return (SignInResult.Failed, false);
        }

        
        public async Task<UserDb?> FindByIdAsync(string id)
        {
            var result = await _userManager.FindByIdAsync(id);

            if (result == null)
            {
                return null;
            }

            return result;
        }
        public  bool BelongsToSection(UserDb user, Sections sections)
        {
           return user.UserSection.HasValue && user.UserSection.Value == sections;  
        }

        public async Task<bool> DeactivateUserAsync(UserDb user)
        {
            if (user == null)
                return false;

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


     public async Task SendTwoFactorCodeAsync(UserDb user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            await _emailService.SendEmailAsync(user.Email??"null", "Your Thinkly 2FA code", $"Your 2FA code is <b>{token}</b>");
        }
        public async Task<IdentityResult> ConfirmEmailAsync(UserDb user, string token)
        {
        return await _userManager.ConfirmEmailAsync(user, token);   
        }
       
        public async Task<SignInResult> VerifyTwoFactorCodeAsync(Verify2FAViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Email);
            if (user == null) return SignInResult.Failed;
            var result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, model.Code, false, false);
            return result;
        }
        public async Task LogoutUserAsync()
        {
             await _signInManager.SignOutAsync();
            
        }
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = $"https://yourdomain.com/Account/ResetPassword?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(
                email,
                "Reset Password",
                $"Click here to reset your password: <a href='{link}'>Reset Password</a>");
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return IdentityResult.Failed();

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            return result;
        }
        public async Task ChangeEmailAsync(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            var link = $"https://yourdomain.com/Account/ConfirmChangeEmail?userId={user.Id}&newEmail={newEmail}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(
                newEmail,
                "Confirm Email Change",
                $"Confirm your new email: <a href='{link}'>Confirm</a>");
        }
        public async Task<IdentityResult> ConfirmChangeEmailAsync(string userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed();
            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            return result;
        }
    }
}
