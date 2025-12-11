using LabBooking.Application.Common.Dtos;

namespace LabBooking.Application.Features.Emails.Commands;

// Command nhận vào Stream (để đọc Excel SV) và Attachment (để gửi kèm)
public class SendScheduleCommand : IRequest<string>
{
    public Stream StudentListStream { get; set; }
    public EmailAttachmentDto Attachment { get; set; }
}