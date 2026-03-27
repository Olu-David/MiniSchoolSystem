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
            // 1. Validation Check
            if (string.IsNullOrEmpty(_emailSettings.Username) || string.IsNullOrEmpty(_emailSettings.Password))
            {
                throw new Exception("SMTP Credentials missing. Check Render Environment Variables.");
            }

            // 2. Setup SMTP Client for Google/Render
            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                UseDefaultCredentials = false, // Critical: Don't use system credentials
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true, // Required for Port 587
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // 3. Create the Message
            var mailMessage = new MailMessage
            {
                // FromEmail must be your Gmail address or Google will block it
                From = new MailAddress(_emailSettings.Username, _emailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"✅ Email successfully sent to {toEmail} via Gmail.");
            }
            catch (SmtpException smtpEx)
            {
                // This catches specific Google SMTP errors (e.g., Auth Failed)
                Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ General Email Error: {ex.Message}");
                throw;
            }
        }
    }
}