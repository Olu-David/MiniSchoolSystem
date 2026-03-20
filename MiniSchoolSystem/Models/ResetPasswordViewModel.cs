using System.ComponentModel.DataAnnotations;

namespace MiniSchoolSystem.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="Enter Email")]
        [EmailAddress]
        public string? Email { get; internal set; }

        [Required(ErrorMessage ="Enter Token")]
        public string? Token { get; internal set; }

        [Required(ErrorMessage ="Enter New Password")]
        public string? NewPassword { get; internal set; }
    }
}