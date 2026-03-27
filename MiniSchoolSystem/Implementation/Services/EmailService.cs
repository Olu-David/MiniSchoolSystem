using Microsoft.Extensions.Options;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;
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
            // The 'Password' field holds your SendGrid API Key (SG.xxx)
            var client = new SendGridClient(_emailSettings.Password);

            // Setup the 'From' (must be verified in SendGrid) and the 'To'
            var from = new EmailAddress(_emailSettings.Email, _emailSettings.DisplayName);
            var to = new EmailAddress(toEmail);

            // Create the email message (Plain text is empty, HTML is 'body')
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);

            try
            {
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Email sent successfully to {toEmail}");
                }
                else
                {
                    // This will show you exactly why SendGrid rejected it (e.g., "Unauthorized")
                    var errorDetails = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"❌ SendGrid Error: {response.StatusCode}. Details: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ System Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}