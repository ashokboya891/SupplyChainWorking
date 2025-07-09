using MimeKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace SupplyChain.BG
{
    public class MailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("aboya7685@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("aboya7685@gmail.com", "hulu annv olsc fwqb");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
