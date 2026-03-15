using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MiniSchoolSystem.Implementation.Settings;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface IEmailService
    {
      
        public Task SendEmailAsync(string toEmail, string Subject, string Body);
    }
}
