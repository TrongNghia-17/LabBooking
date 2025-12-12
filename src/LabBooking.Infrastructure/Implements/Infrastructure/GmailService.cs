using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LabBooking.Infrastructure.Implements.Infrastructure
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailService(IConfiguration config) => _config = config;

        public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
        {
            var settings = _config.GetSection("MailSettings");
            var host = settings["Host"]; // smtp.gmail.com
            var port = int.Parse(settings["Port"]); // 587
            var mail = settings["Mail"];
            var password = settings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings["DisplayName"] ?? "LabBooking", mail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            if (attachment != null && attachment.FileContent != null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
            }
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            // Tăng Timeout lên 60 giây (Mạng Render free rất chậm)
            client.Timeout = 60000;

            try
            {
                // Kết nối: CHỈ ĐỊNH RÕ RÀNG StartTls
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                // Đăng nhập
                await client.AuthenticateAsync(mail, password);

                // Gửi
                await client.SendAsync(message);
                Console.WriteLine($"[Success] Email sent to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Mail Error] {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}