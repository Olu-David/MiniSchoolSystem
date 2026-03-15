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
        private readonly UserManager<UserDb> _UserManager;
        private readonly EmailSettings _EmailSettings;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public EmailService(UserManager<UserDb> userManager, IOptions<EmailSettings> emailSettings, RoleManager<IdentityRole> roleManager)
        {
            _UserManager = userManager;
            _EmailSettings = emailSettings.Value;
            _RoleManager = roleManager;
        }

        public async Task SendEmailAsync(string toEmail, string Subject, string Body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            try
            {
                smtpClient.Credentials = new NetworkCredential("Davveeyolusanya1@gmail.com", "eatmctvyoethjtvj");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                smtpClient.Port = 587;
                smtpClient.Host = "Smtp.gmail.com";
                smtpClient.UseDefaultCredentials = false;

                var mail = new MailMessage
                {
                    From = new MailAddress("Davveeyolusanya1@gmail.com"),
                    Subject = Subject,
                    Body = Body,
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.Delay | DeliveryNotificationOptions.OnFailure



                };
                mail.To.Add(toEmail);
                await smtpClient.SendMailAsync(mail);
            }
            catch
            {

                smtpClient.Timeout = 15000;
                smtpClient.Dispose();
            }
        }
    }
}

   
