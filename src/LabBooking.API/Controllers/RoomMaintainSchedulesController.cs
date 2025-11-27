using LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.DeleteRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateStatus;
using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;
using LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetAllRoomMaintainSchedules;
using LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetRoomMaintainScheduleById;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomMaintainSchedulesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new room maintain schedule
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager")]
    // Sửa kiểu trả về trong Swagger là RoomMaintainScheduleResponse
    [ProducesResponseType(typeof(RoomMaintainScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomMaintainScheduleResponse>> Create([FromBody] CreateRoomMaintainScheduleCommand command)
    {
        // result bây giờ là full object data
        var result = await mediator.Send(command);

        // Trả về 201 Created cùng với data
        // Hàm này sẽ tạo header Location trỏ tới hàm GetById và body là result
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get all room maintain schedules with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering schedules</param>
    /// <returns>List of schedules</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Manager")]
    [ProducesResponseType(typeof(PagedResult<RoomMaintainScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<RoomMaintainScheduleResponse>>> GetAll([FromQuery] GetAllRoomMaintainSchedulesQuery query)
    {
        var schedules = await mediator.Send(query);
        return Ok(schedules);
    }

    /// <summary>
    /// Delete an existing room maintain schedule
    /// </summary>
    /// <param name="id">The Id of the schedule to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteRoomMaintainScheduleCommand(id);
        await mediator.Send(command);

        return NoContent(); // Trả về 204 No Content
    }

    /// <summary>
    /// Update an existing room maintain schedule
    /// </summary>
    /// <param name="id">The Id of the schedule to update</param>
    /// <param name="command">The new data for the schedule</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateRoomMaintainScheduleCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Update only the status of a room maintain schedule (e.g., Mark as Done)
    /// </summary>
    [HttpPatch("{id:guid}/status")] // Route: api/RoomMaintainSchedules/{id}/status
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateRoomMaintainStatusCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get a specific room maintain schedule by Id
    /// </summary>
    /// <param name="id">The Id of the schedule</param>
    /// <returns>The room maintain schedule</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomMaintainScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomMaintainScheduleResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetRoomMaintainScheduleByIdQuery(id);
        var schedule = await mediator.Send(query);

        return Ok(schedule);
    }
}
