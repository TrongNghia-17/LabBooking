using LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomChecksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Bảo vệ gửi báo cáo kiểm tra phòng (Check-in/Check-out).
    /// <para>Nếu có thiết bị hỏng, hệ thống sẽ tự động tạo Sự cố (Incident).</para>
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SecurityGuard")] // Chỉ bảo vệ mới được làm
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateRoomCheckCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }
}
