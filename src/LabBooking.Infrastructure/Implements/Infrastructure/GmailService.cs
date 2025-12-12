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
            var host = settings["Host"];        // smtp.gmail.com
            var port = int.Parse(settings["Port"]); // Bắt buộc là 465
            var mail = settings["Mail"];        // Gmail của bạn
            var password = settings["Password"]; // App Password 16 ký tự

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
            client.Timeout = 30000; // 30s

            try
            {
                // --- ĐOẠN QUAN TRỌNG NHẤT VỚI GMAIL TRÊN RENDER ---
                // Port 465 BẮT BUỘC dùng SslOnConnect
                await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);

                // Đăng nhập
                await client.AuthenticateAsync(mail, password);

                await client.SendAsync(message);
                Console.WriteLine($"[Success] Sent to {to} via Gmail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Gmail Error] {ex.Message}");
                throw;
            }
            finally
            {
                if (client.IsConnected) await client.DisconnectAsync(true);
            }
        }
    }
}