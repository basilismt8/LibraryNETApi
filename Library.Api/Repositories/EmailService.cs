using System.Net.Mail;
using System.Net;

namespace Library.Api.Repositories
{
    public class EmailService: IEmailServiceInterface
    {
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.yourprovider.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your_email@example.com", "password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage("your_email@example.com", toEmail, subject, message);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
