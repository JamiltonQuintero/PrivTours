using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PrivTours.Models.Business
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration; //set only via Secret Manager
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(configuration.GetValue<string>("SendGridKey"), subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            /*var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("masuarez765@gmail.com", "Password Recovery"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);*/

            SmtpClient client = new SmtpClient(configuration.GetValue<string>("mailSettings:smtp:host"), configuration.GetValue<int>("mailSettings:smtp:port"));
            client.Credentials = new NetworkCredential(configuration.GetValue<string>("mailSettings:smtp:username"), configuration.GetValue<string>("mailSettings:smtp:password"));
            client.EnableSsl = configuration.GetValue<bool>("mailSettings:smtp:enableSsl");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(configuration.GetValue<string>("mailSettings:smtp:from"), configuration.GetValue<string>("mailSettings:smtp:name_from"));
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            client.Send(mailMessage);

            return Task.FromResult(0);
        }
    }
}
