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
            // 🔐 Safety check (VERY IMPORTANT)
            if (string.IsNullOrEmpty(_emailSettings.Email) ||
                string.IsNullOrEmpty(_emailSettings.Password) ||
                string.IsNullOrEmpty(_emailSettings.Host))
            {
                throw new Exception("Email settings not configured properly");
            }

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port);

            client.Credentials = new NetworkCredential(
                _emailSettings.Email,
                _emailSettings.Password
            );

            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("✅ Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email Failed: {ex.Message}");
                throw;
            }
        }
    }
}
