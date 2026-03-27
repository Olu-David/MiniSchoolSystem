namespace MiniSchoolSystem.Implementation.Settings
{
    public class EmailSettings
    {
        public string Email { get; set; } = default!;        // sender email
        public string Password { get; set; } = default!;     // app password
        public string Host { get; set; } = default!;         // SMTP host
        public int Port { get; set; }                        // SMTP port
        public string DisplayName { get; set; } = "Mini School System"; // sender name
    }
    }
}
