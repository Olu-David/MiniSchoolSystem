using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Implementation.Services
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

        public bool BelongToASection(UserDb user, Sections sections)
        {
            return user.UserSection.HasValue && sections == user.UserSection.Value;
        }

        public async Task<IdentityResult> ConfirmMailAsync(UserDb user, string? token)
        {
            if (token == null) return IdentityResult.Failed(new IdentityError { Description="Token Failed"});

            var result= await _userManager.ConfirmEmailAsync(user, token);  
            if(!result.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Account Confirmation Failed" });
            }
            return IdentityResult.Success;
        }

        public async Task<bool> DeactivateAccountAsync(UserDb user)
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            var Deactivate = await _userManager.UpdateAsync(user);
            return Deactivate.Succeeded;

        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Resseting Password for User");
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("Email Found Empty, Try Again");
                throw new ArgumentNullException(nameof(email), "Email input cannot be empty.");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(user.Email ?? "null", "ForgotPassword", $"Your Reset Code is <b>{token}</b>");

            return true;

        }

        public async Task<(SignInResult Result, bool requires2FA)> LoginUserAsync(LoginViewDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email??"null");
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user)) return (SignInResult.Failed, false);

            var result = await _signInManager.PasswordSignInAsync(user, model.Password??"", false, lockoutOnFailure: false);
            if (result.RequiresTwoFactor)
            {
                await Send2FAAsync(user);
                return (SignInResult.TwoFactorRequired, true);
            }
            if (result.Succeeded)
            {
                return (SignInResult.Success, false);
                
            }
            return (SignInResult.Failed, false);


        }

        public async Task<IdentityResult> RegistrationAsync(RegisterViewModel model, string? ConfirmationLink)
        {
            // 1. Create the User object from the ViewModel
            var user = new UserDb
            {
                UserName = model.Email, // Identity requires a UserName
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                UserSection = model.Role == "Student" ? model.Section : null,
                EmailConfirmed = false // Keep them locked out until they click the link
            };

            // 2. Save to Database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 3. Handle Roles (Student/Parent/etc.)
                if (!string.IsNullOrEmpty(model.Role))
                {
                    // Create role if it doesn't exist (Safety Check)
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    // Assign the role
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
            }

            return result;
        }
          
        
        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string password)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token), "Token is missing.");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("New password is required.", nameof(password));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            var result = await _userManager.ResetPasswordAsync(user, email, password);
            return result;
        }

        public async Task Send2FAAsync(UserDb user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "Token is missing.");

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            await _emailService.SendEmailAsync(user.Email ?? "null", "TwoFactorAuthenticator Code", $"Your Code is {token}");
        }

        public async Task<SignInResult> Verify2FAAsync(Verify2FAViewModel model)
        {
            if (model.Email == null)
                if (model.Email == null) throw new ArgumentNullException(nameof(model.Email), "Token is missing.");

            var result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, model.Code, false, false);
            return result;



        }
        public async Task<(bool Success, string Message)> SetUserLockoutAsync(string userId, bool shouldLock)
        {
            // 1. Find the user in the identity database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            // 2. Prevent SuperAdmin from locking themselves out (Safety Check)
            // Assuming you have a way to get the current logged-in user's ID
            // if (user.Id == currentAdminId) return (false, "You cannot lock yourself out.");

            IdentityResult result;

            if (shouldLock)
            {
                // Lock the user until the year 9999 (basically forever)
                result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            else
            {
                // Unlock the user by removing the expiration date
                result = await _userManager.SetLockoutEndDateAsync(user, null);
            }

            if (result.Succeeded)
            {
                string status = shouldLock ? "locked" : "unlocked";
                return (true, $"User has been successfully {status}.");
            }

            return (false, "Failed to update lockout status.");
        }
    }
}

       

