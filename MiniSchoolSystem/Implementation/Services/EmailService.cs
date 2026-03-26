using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
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
            // NOTE: The 'From' email must be the one you verified in SendGrid (Single Sender)
            message.From.Add(new MailboxAddress(_EmailSettings.DisplayName, _EmailSettings.Email ?? ""));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Timeout = 10000; // 10 seconds

                    // Essential for cloud environments like Render
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // SendGrid SMTP Settings
                    // Host: smtp.sendgrid.net | Port: 587 (Standard for StartTls)
                    await client.ConnectAsync("smtp.sendgrid.net", 2525, SecureSocketOptions.StartTls);

                    // CRITICAL: Username is ALWAYS "apikey". 
                    // Password is your SG.xxxxxxxx API Key from SendGrid dashboard.
                    await client.AuthenticateAsync(_EmailSettings.Password, _EmailSettings.Password);

                    await client.SendAsync(message);
                    Console.WriteLine("✅ SENDGRID: Mail sent successfully to " + toEmail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ SENDGRID ERROR: {ex.Message}");
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