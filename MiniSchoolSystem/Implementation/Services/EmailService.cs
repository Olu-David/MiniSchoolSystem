using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MimeKit;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Settings;

namespace MiniSchoolSystem.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _EmailSettings;


        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _EmailSettings = emailSettings.Value;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_EmailSettings.DisplayName, _EmailSettings.Email ?? ""));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            // Using BodyBuilder handles the HTML formatting properly
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    // 1. IMPORTANT: Bypass certificate validation for the server environment
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // 2. Use Port 465 + SslOnConnect (Best for Render)
                    // Ensure _EmailSettings.Port is 465 and Host is "smtp.gmail.com"
                    await client.ConnectAsync(_EmailSettings.Host, 465, SecureSocketOptions.SslOnConnect);

                    // 3. Authenticate using your App Password
                    await client.AuthenticateAsync(_EmailSettings.Email, _EmailSettings.Password);

                    await client.SendAsync(message);

                    // 4. Use Console.WriteLine so it shows up in Render Logs
                    Console.WriteLine($"✅ MailKit: Mail sent successfully to {toEmail}");
                }
                catch (Exception ex)
                {
                    // This will now show up in your Render Dashboard Logs
                    Console.WriteLine($"❌ SMTP ERROR: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"INNER ERROR: {ex.InnerException.Message}");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}