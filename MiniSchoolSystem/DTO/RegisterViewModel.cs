using Microsoft.AspNetCore.Identity;
using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniSchoolSystem.DTO

    {
        public class RegisterViewModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please select a role")]
            public string Role { get; set; } = string.Empty; // "Student", "Teacher", or "Parent"

            // This is the field that caused the string conversion error earlier.
            // Keeping it as a string here makes it compatible with HTML <select> tags.
            public Sections Section { get; set; }
        
        [Phone, Required(ErrorMessage ="Enter PhoneNumber")]
        public string? PhoneNumber {  get; set; }
        }
    }

