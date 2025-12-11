using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace LabBooking.Infrastructure.Implements.Infrastructure;

public class GmailService(
    ILogger<GmailService> logger,
    IConfiguration config) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
    {
        if (string.IsNullOrWhiteSpace(to)) return;

        try
        {
            var settings = config.GetSection("MailSettings");
            var client = new SmtpClient(settings["Host"], int.Parse(settings["Port"]))
            {
                Credentials = new NetworkCredential(settings["Mail"], settings["Password"]),
                EnableSsl = true
            };

            var message = new MailMessage(settings["Mail"], to, subject, body) { IsBodyHtml = true };

            if (attachment != null && attachment.FileContent != null)
            {
                var ms = new MemoryStream(attachment.FileContent);
                message.Attachments.Add(new Attachment(ms, attachment.FileName, attachment.ContentType));
            }

            await client.SendMailAsync(message);
        }
        catch (FormatException ex)
        {
            // Bắt lỗi định dạng email: Ghi log và BỎ QUA, không ném lỗi ra ngoài
            // Để Hangfire coi như job này đã xong (dù gửi thất bại), không retry nữa.
            logger.LogError($"[Lỗi Email] Bỏ qua email '{to}' vì sai định dạng: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Các lỗi khác (như mất mạng, sai pass) thì ném ra để Hangfire retry
            throw;
        }
    }
}
