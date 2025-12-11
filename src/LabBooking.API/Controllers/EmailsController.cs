using LabBooking.Application.Features.Emails.Commands;
using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailsController(IMediator mediator) : ControllerBase
{
    [HttpPost("send-custom-email")]
    public async Task<IActionResult> SendCustomEmail([FromForm] SendEmailRequestDto request)
    {
        // Validate
        if (request.StudentFile == null || request.StudentFile.Length == 0)
            return BadRequest("Vui lòng upload file danh sách sinh viên.");

        // Mapping (Giờ chỉ còn 1 dòng logic xử lý file đính kèm)
        var attachmentDto = await request.AttachmentFile.ToAttachmentDtoAsync();

        // Mapping Command
        var command = new SendScheduleCommand
        {
            StudentListStream = request.StudentFile.OpenReadStream(), // Lưu ý: Stream này sẽ được Dispose bên trong Handler hoặc Command
            Attachment = attachmentDto,
            Subject = request.Subject,
            BodyTemplate = request.Body
        };

        var result = await mediator.Send(command);
        return Ok(new { Message = result });
    }
}
