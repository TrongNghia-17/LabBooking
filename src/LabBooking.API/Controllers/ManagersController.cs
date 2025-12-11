using LabBooking.Application.Common.Dtos;
using LabBooking.Application.Features.Emails.Commands;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy thông tin phòng Lab và thiết bị chưa bảo trì thuộc quyền quản lý của Manager hiện tại.
    /// </summary>
    [HttpGet("lab-details")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ManagerLabDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ManagerLabDetailsResponse>> GetManagerLabDetails()
    {
        var query = new GetManagerLabDetailsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("send-custom-email")]
    public async Task<IActionResult> SendCustomEmail([FromForm] SendEmailRequestDto request)
    {
        // Validate cơ bản
        if (request.StudentFile == null || request.StudentFile.Length == 0)
            return BadRequest("Vui lòng upload file danh sách sinh viên.");

        // 1. Xử lý Attachment (nếu có)
        EmailAttachmentDto attachmentDto = null;
        if (request.AttachmentFile != null)
        {
            using var ms = new MemoryStream();
            await request.AttachmentFile.CopyToAsync(ms);
            attachmentDto = new EmailAttachmentDto
            {
                FileName = request.AttachmentFile.FileName,
                ContentType = request.AttachmentFile.ContentType,
                FileContent = ms.ToArray()
            };
        }

        // 2. Xử lý file danh sách (StudentFile)
        using var studentStream = request.StudentFile.OpenReadStream();

        // 3. Tạo Command (Mapping từ DTO sang Command)
        var command = new SendScheduleCommand
        {
            StudentListStream = studentStream,
            Attachment = attachmentDto,
            Subject = request.Subject,
            BodyTemplate = request.Body
        };

        var result = await mediator.Send(command);
        return Ok(new { Message = result });
    }
}
