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

    [HttpPost("send-schedule")]
    public async Task<IActionResult> SendSchedule(IFormFile studentFile, IFormFile scheduleFile)
    {
        if (studentFile == null || scheduleFile == null)
            return BadRequest("Thiếu file!");

        // 1. Chuyển file đính kèm sang Byte Array (Để lưu trữ được)
        using var msSchedule = new MemoryStream();
        await scheduleFile.CopyToAsync(msSchedule);

        var attachmentDto = new EmailAttachmentDto
        {
            FileName = scheduleFile.FileName,
            ContentType = scheduleFile.ContentType,
            FileContent = msSchedule.ToArray()
        };

        // 2. Mở stream file danh sách (để đọc ngay)
        using var msStudents = studentFile.OpenReadStream();

        // 3. Tạo Command
        var command = new SendScheduleCommand
        {
            StudentListStream = msStudents,
            Attachment = attachmentDto
        };

        // 4. Gửi cho MediatR xử lý
        var result = await mediator.Send(command);

        return Ok(new { Message = result });
    }
}
