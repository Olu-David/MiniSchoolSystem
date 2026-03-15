namespace MiniSchoolSystem.Implementation.Interfaces
{
    public class ResetPasswordViewModel
    {
        public string Email { get; internal set; }
        public string Token { get; internal set; }
        public string NewPassword { get; internal set; }
    }
}