namespace MiniSchoolSystem.DTO
{
    public class LoginViewDTO
    {
        public LoginViewDTO(string email, string password, bool rememberMe)
        {
            Email = email;
            Password = password;
            RememberMe = rememberMe;
        }

        public string Email { get; set; }
        public string Password { get; set; }   
        public bool RememberMe { get; set; }
    }
}
