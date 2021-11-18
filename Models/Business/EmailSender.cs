using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using PrivTours.Models.Entities;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PrivTours.Models.Business
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;
        private readonly MailSettings mailSettings;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.mailSettings = new MailSettings();
            configuration.GetSection("mailSettings").Bind(mailSettings);
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                Credentials = new NetworkCredential(mailSettings.Username, mailSettings.Password),
                EnableSsl = mailSettings.EnableSsl
            };
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(mailSettings.From, mailSettings.NameFrom)
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            client.Send(mailMessage);

            return Task.FromResult(0);
        }
    }
}
