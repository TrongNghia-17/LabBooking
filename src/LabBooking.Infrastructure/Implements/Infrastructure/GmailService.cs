using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit; // Nhớ thêm dòng này để dùng IProtocolLogger
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
            var host = settings["Host"];
            var port = int.Parse(settings["Port"]);
            var mailAddress = settings["Mail"];
            var password = settings["Password"];
            var displayName = settings["DisplayName"] ?? "LabBooking System";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, mailAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            if (attachment != null && attachment.FileContent != null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
            }
            message.Body = builder.ToMessageBody();

            // SỬ DỤNG PROTOCOL LOGGER ĐỂ IN LOG CHI TIẾT RA CONSOLE
            using var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));

            client.CheckCertificateRevocation = false;
            client.Timeout = 10000; // Giảm xuống 10s cho nhanh thấy lỗi

            try
            {
                Console.WriteLine($"[Debug] Bắt đầu kết nối đến {host}:{port}...");

                // Kết nối
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                Console.WriteLine("[Debug] Kết nối thành công.");

                // Đăng nhập
                await client.AuthenticateAsync(mailAddress, password);
                Console.WriteLine("[Debug] Đăng nhập thành công.");

                // Gửi
                await client.SendAsync(message);
                Console.WriteLine($"[Success] Đã gửi mail cho {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected) await client.DisconnectAsync(true);
            }
        }
    }
}