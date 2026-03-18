using Microsoft.AspNetCore.Identity;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegistrationAsync(RegisterViewModel model, string? ConfirmationLink);
        Task<(SignInResult Result, bool requires2FA)> LoginUserAsync(LoginViewDTO model);
        Task<IdentityResult> ConfirmMailAsync(UserDb user, string? token);
        Task<SignInResult> Verify2FAAsync(Verify2FAViewModel model);
        Task Send2FAAsync(UserDb user);
        Task<bool> DeactivateAccountAsync(UserDb user);
        Task<bool> ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string password);
        bool BelongToASection(UserDb user, Sections sections);

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                

    }
}
