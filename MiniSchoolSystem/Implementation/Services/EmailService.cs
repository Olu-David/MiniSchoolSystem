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
            message.From.Add(new MailboxAddress(_EmailSettings.DisplayName, _EmailSettings.Email??""));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            // Using BodyBuilder handles the HTML formatting properly
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    // For Gmail, StartTls is the standard for Port 587
                    // Use 465 and SslOnConnect for Render
                    await client.ConnectAsync(_EmailSettings.Host, 465, SecureSocketOptions.SslOnConnect);
                    // Note: Use your App Password here
                    await client.AuthenticateAsync(_EmailSettings.Email, _EmailSettings.Password);
                    

                    await client.SendAsync(message);

                    System.Diagnostics.Debug.WriteLine("✅ MailKit: Mail sent successfully to " + toEmail);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("❌ MailKit Error: " + ex.Message);
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