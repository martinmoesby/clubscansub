using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ClubScansub.Service.Communication
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }

        private readonly SmtpClient client;

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
            client = new SmtpClient()
            {
                Host = Options.Host,
                Port = Options.Port,
                EnableSsl = Options.UseSSL,
                Credentials = new NetworkCredential(Options.UserName, Options.Password)
            };

        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage(Options.From, email);
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            mailMessage.Body = message;

            return client.SendMailAsync(mailMessage);
        }

    }
}
