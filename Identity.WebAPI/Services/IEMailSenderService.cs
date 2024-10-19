using MailKit.Net.Smtp;
using MimeKit;

namespace Identity.WebAPI.Services
{
    public interface IEMailSenderService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
    public class EMailSenderService : IEMailSenderService
    {
        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Identity App", "****************"));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("****************", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("****************", "****************");
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Email Error: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
