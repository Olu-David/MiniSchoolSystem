namespace MiniSchoolSystem.Implementation.Settings
{
    public class EmailSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587; // Required for Google TLS
        public string Username { get; set; } = string.Empty; // Your Gmail
        public string Password { get; set; } = string.Empty; // 16-char App Password
        public string FromEmail { get; set; } = string.Empty; // Usually same as Username
        public string DisplayName { get; set; } = "Mini School System";
    }
}