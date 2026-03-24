using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Settings;
using MiniSchoolSystem.Models;
using System.Net;
using System.Net.Mail;

namespace MiniSchoolSystem.Implementation.Services
{
    public class EmailService : IEmailService
    {
       
        private readonly EmailSettings _EmailSettings;


        public EmailService(UserManager<UserDb> userManager, IOptions<EmailSettings> emailSettings, RoleManager<IdentityRole> roleManager)
        {
            
            _EmailSettings = emailSettings.Value;
            
        }

        public async Task SendEmailAsync(string toEmail, string Subject, string Body)
        {
            // Use 'using' so the connection closes properly and doesn't hang
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                try
                {
                    // 1. Set these FIRST
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("Davveeyolusanya1@gmail.com", "eatmctvyoethjtvj");
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 20000; // 20 seconds is enough

                    var mail = new MailMessage
                    {
                        From = new MailAddress("Davveeyolusanya1@gmail.com", "SabiSchool System"),
                        Subject = Subject,
                        Body = Body,
                        IsBodyHtml = true
                    };
                    mail.To.Add(toEmail);

                    // 2. Await the send
                    await smtpClient.SendMailAsync(mail);
                    System.Diagnostics.Debug.WriteLine("✅ Mail sent to " + toEmail);
                }
                catch (Exception ex)
                {
                    // 3. DON'T HIDE THE ERROR! 
                    // Look at your 'Output' window in Visual Studio to see this:
                    System.Diagnostics.Debug.WriteLine("❌ SMTP Error: " + ex.Message);
                    throw; // This lets the app know something went wrong
                }
            }
        }
    }
}


