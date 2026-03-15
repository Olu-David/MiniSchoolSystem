namespace MiniSchoolSystem.DTO
{
    public class Verify2FAViewModel
    {
        public Verify2FAViewModel(string email, string code)
        {
            Email = email;
            Code = code;
        }

        public string Email { get; set; } 
        public string Code { get; set; }

    }
}
