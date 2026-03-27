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
    using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
    {
        client.Credentials = new NetworkCredential(
            _emailSettings.Email,
            _emailSettings.Password
        );

        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.Email, "Mini School System"),
            Subject = subject,
            Body = body, // ✅ using "body" now
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email Failed: {ex.Message}");
            throw;
        }
    }
}
