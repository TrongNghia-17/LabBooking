using Microsoft.AspNetCore.Http;

namespace LabBooking.Application.Features.Emails.Dtos;

public class SendEmailRequestDto
{
    // File danh sách sinh viên (Bắt buộc)
    public IFormFile StudentFile { get; set; }

    // File đính kèm (Có thể null)
    public IFormFile? AttachmentFile { get; set; }

    // Tiêu đề mail
    public string Subject { get; set; }

    // Nội dung mail (HTML)
    public string Body { get; set; }
}