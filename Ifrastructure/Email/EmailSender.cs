using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;

namespace Ifrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string msg)
        {
            var client = new SendGrid.SendGridClient(_configuration["SendGrid:ApiKey"]);
            var message = new SendGridMessage
            {
                From = new EmailAddress("alexlux@live.com", _configuration["SendGrid:User"]),
                Subject = subject,
                PlainTextContent = msg,
                HtmlContent = msg
            };

            message.AddTo(new EmailAddress(email));

            message.SetClickTracking(false, false);

            await client.SendEmailAsync(message);

            
        }
    }
}