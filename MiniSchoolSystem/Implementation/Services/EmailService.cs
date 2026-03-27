using Microsoft.Extensions.Options;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Settings;
using System.Net;
using System.Net.Mail;

namespace MiniSchoolSystem.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 🔐 Safety check for missing credentials
            if (string.IsNullOrEmpty(_emailSettings.Username) ||
                string.IsNullOrEmpty(_emailSettings.Password))
            {
                throw new Exception("Email credentials (Username/Password) are missing. Check Render Env Variables.");
            }

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port);

            // Required for Mailtrap and most cloud SMTP servers
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("✅ Email sent to Mailtrap successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SMTP Error: {ex.Message}");
                throw;
            }
        }
    }
}