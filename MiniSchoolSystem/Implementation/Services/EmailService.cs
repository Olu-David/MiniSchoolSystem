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
                    // Force the timeout to be shorter so you don't wait forever
                    client.Timeout = 10000; // 10 seconds

                    // Bypass all certificate checks (Essential for Render/Linux)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Use Port 465 - Render usually leaves this open
                    await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    // Authenticate with your App Password
                    await client.AuthenticateAsync(_EmailSettings.Email, _EmailSettings.Password);

                    await client.SendAsync(message);
                    Console.WriteLine("✅ EMAIL SENT SUCCESSFULLY!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ SMTP ERROR: {ex.Message}");
                    // This tells us if it's a network block or a protocol error
                    if (ex is System.Net.Sockets.SocketException)
                        Console.WriteLine("REASON: Render is blocking this Port/IP.");
                }
            }
        }
    }
}
