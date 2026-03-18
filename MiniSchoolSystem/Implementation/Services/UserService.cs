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

        public Task<IdentityResult> ConfirmMailAsync(UserDb user, string? token)
        {
            throw new NotImplementedException();
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user)) return (SignInResult.Failed, false);

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
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
            var user = new UserDb
            {
                Email = model.Email,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                FullName = $"{model.FirstName} {model.LastName}",
                UserSection = model.Role == "Student" ? model.Section : null,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.Role == "Student" || model.Role == "Parent")
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));

                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }


            }
            if (!string.IsNullOrEmpty(ConfirmationLink))
            {
                await _emailService.SendEmailAsync(user.Email!, "Email Confirmation",
                    $"Confirm your Account By clicking this link <a href='{ConfirmationLink}'>Click Here</a>");
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
    }
}

       

