using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.DeleteDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;
using LabBooking.Application.Features.DoorRequests.Dtos;
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
    /// Lấy danh sách yêu cầu mở cửa (Dành cho Manager, có filter, paging)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(PagedResult<DoorRequestDto>), StatusCodes.Status200OK)]
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
}
