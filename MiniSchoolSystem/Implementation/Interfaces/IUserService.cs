using Microsoft.AspNetCore.Identity;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        bool BelongsToSection(UserDb user, Sections sections);
        Task<bool>DeactivateUserAsync(UserDb user);
        Task<(SignInResult result, bool requires2FA )>LoginUserAsync(LoginViewDTO model);
        Task<IdentityResult> ConfirmEmailAsync(UserDb user, string token);
        Task SendTwoFactorCodeAsync(UserDb user);
        Task<SignInResult> VerifyTwoFactorCodeAsync(Verify2FAViewModel model);
        Task LogoutUserAsync();
        Task<UserDb?>FindByIdAsync(string Userid);
         Task ForgotPasswordAsync(string email);

         Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);

        Task ChangeEmailAsync(string userId, string newEmail);

          

    }
}
