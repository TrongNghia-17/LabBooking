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

            using var client = new SmtpClient();

            // QUAN TRỌNG: Tắt kiểm tra thu hồi chứng chỉ
            // Trên môi trường Container/Render, việc này hay gây ra Timeout do network restrictions
            client.CheckCertificateRevocation = false;

            // Timeout kết nối
            client.Timeout = 30000; // 30s

            try
            {
                // Logic chọn Port và SSL
                SecureSocketOptions socketOptions;

                if (port == 465)
                {
                    socketOptions = SecureSocketOptions.SslOnConnect;
                }
                else if (port == 587)
                {
                    socketOptions = SecureSocketOptions.StartTls;
                }
                else
                {
                    // Trường hợp dự phòng nếu cấu hình sai port
                    socketOptions = SecureSocketOptions.Auto;
                }

                Console.WriteLine($"[Mail Info] Connecting to {host}:{port} using {socketOptions}...");

                await client.ConnectAsync(host, port, socketOptions);
                Console.WriteLine("[Mail Info] Connected. Authenticating...");

                // Đăng nhập
                await client.AuthenticateAsync(mailAddress, password);
                Console.WriteLine("[Mail Info] Authenticated. Sending...");

                // Gửi
                await client.SendAsync(message);
                Console.WriteLine($"[Success] Email sent to {to} via Gmail");
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi để debug dễ hơn
                Console.WriteLine($"[Mail Error] Host: {host} | Port: {port} | Error: {ex.Message}");
                Console.WriteLine($"[Stack Trace] {ex.StackTrace}");
                throw;
            }
            finally
            {
                if (client.IsConnected) await client.DisconnectAsync(true);
            }
        }
    }
}