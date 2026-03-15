namespace MiniSchoolSystem.Implementation.Settings
{
    public class JwtSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }   
        public DateTime ExpiryMinutes { get; set; }
    }
}
