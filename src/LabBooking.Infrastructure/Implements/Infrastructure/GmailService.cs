using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit.Net.Smtp;                   // Dùng của MailKit
using MailKit.Security;                   // Dùng Security của MailKit
using MimeKit;                            // Dùng MimeKit để tạo nội dung mail

namespace LabBooking.Infrastructure.Implements.Infrastructure
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
        {
            var settings = _config.GetSection("MailSettings");

            // 1. Tạo message bằng MimeKit (Hiện đại hơn)
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings["DisplayName"] ?? "LabBooking System", settings["Mail"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            // 2. Tạo Body Builder (Hỗ trợ HTML và File đính kèm dễ dàng)
            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            // Đính kèm file nếu có
            if (attachment != null && attachment.FileContent != null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
            }

            message.Body = builder.ToMessageBody();

            // 3. Sử dụng SmtpClient của MailKit (Không phải System.Net.Mail)
            using var client = new SmtpClient();

            try
            {
                // Connect: Dùng Port 587 và SecureSocketOptions.StartTls
                // Quan trọng: smtp.gmail.com đôi khi trả về IPv6 gây lỗi trên Render, 
                // MailKit tự động xử lý tốt hơn, nhưng nếu vẫn lỗi có thể thử hardcode IP (ít khi cần).
                await client.ConnectAsync(settings["Host"], int.Parse(settings["Port"]), SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(settings["Mail"], settings["Password"]);

                // Gửi
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MailKit Error] {ex.Message}");
                throw; // Ném lỗi để Hangfire biết mà retry hoặc log lại
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}