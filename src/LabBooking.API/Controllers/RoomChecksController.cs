using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;
using LabBooking.Application.Features.RoomChecks.Commands.DeleteRoomCheck;
using LabBooking.Application.Features.RoomChecks.Dtos;
using LabBooking.Application.Features.RoomChecks.Queries.GetRoomChecks;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomChecksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Bảo vệ gửi báo cáo kiểm tra phòng (Check-in/Check-out).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SecurityGuard")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateRoomCheckCommand command)
    {
        var id = await mediator.Send(command);

        // FIX: Trả về 201 Created. 
        // Frontend sẽ dùng ID này để điền vào form "Tạo sự cố" nếu cần.
        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    /// <summary>
    /// Xóa phiếu kiểm tra (Soft Delete).
    /// <para>Chỉ xóa được khi phiếu này KHÔNG gắn với sự cố nào.</para>
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SecurityGuard")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Trả về nếu đang dính Incident
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteRoomCheckCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách lịch sử kiểm tra phòng.
    /// <para>- Bảo vệ: Xem lịch sử mình đã check.</para>
    /// <para>- Manager: Xem lịch sử các phòng mình quản lý.</para>
    /// </summary>
    [HttpGet]
    [Authorize] // Cho phép cả Guard và Manager
    [ProducesResponseType(typeof(PagedResult<RoomCheckDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetRoomChecksQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
