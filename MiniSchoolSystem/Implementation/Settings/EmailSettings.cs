namespace MiniSchoolSystem.Implementation.Settings
{
    public class EmailSettings
    {
        public string Host { get; set; } = "sandbox.smtp.mailtrap.io";
        public int Port { get; set; } = 2525;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = "no-reply@minischoolsystem.com";
        public string DisplayName { get; set; } = "Mini School System";
    }
}