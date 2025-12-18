using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.DeleteDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;
using LabBooking.Application.Features.DoorRequests.Dtos;
using LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestDetail;
using LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequests;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoorRequestsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Tạo yêu cầu mở cửa cho một Booking (Chỉ chủ booking mới được tạo)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Lecturer, Student")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDoorRequestCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    /// <summary>
    /// Hủy yêu cầu mở cửa (Chỉ áp dụng cho yêu cầu đang chờ duyệt)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Lecturer, Student")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteDoorRequestCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách yêu cầu mở cửa (Đa năng).
    /// <para>- Nếu là Manager: Xem các yêu cầu gửi đến phòng Lab mình quản lý.</para>
    /// <para>- Nếu là Student/Lecturer: Xem danh sách yêu cầu do chính mình tạo.</para>
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<DoorRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<DoorRequestDto>>> GetAll([FromQuery] GetDoorRequestsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDoorRequestStatusDto requestBody)
    {
        var command = new UpdateDoorRequestStatusCommand(
            id,
            requestBody.NewStatus,
            requestBody.Note
        );

        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Xem chi tiết yêu cầu mở cửa (Dành cho Manager)
    /// </summary>
    /// <param name="id">ID của Door Request</param>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(DoorRequestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)] // Lỗi không đủ quyền
    [ProducesResponseType(StatusCodes.Status404NotFound)]  // Lỗi không tìm thấy
    public async Task<ActionResult<DoorRequestDetailDto>> GetById(Guid id)
    {
        var query = new GetDoorRequestDetailQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
