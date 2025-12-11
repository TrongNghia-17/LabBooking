using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.Application.Features.Emails.Commands;

// Command nhận vào Stream (để đọc Excel SV) và Attachment (để gửi kèm)
public class SendScheduleCommand : IRequest<string>
{
    public Stream StudentListStream { get; set; }
    public EmailAttachmentDto Attachment { get; set; }
    public string Subject { get; set; } // Tiêu đề mail
    public string BodyTemplate { get; set; } // Nội dung chứa {{FullName}}
}