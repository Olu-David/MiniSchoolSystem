namespace MiniSchoolSystem.DTO
{
    public class Verify2FAViewModel
    {
       

        public string? Email { get; set; } 
        public string? token { get; set; }
        public bool RememberMe { get; internal set; }
        public bool RememberClient { get; internal set; }
    }
}
