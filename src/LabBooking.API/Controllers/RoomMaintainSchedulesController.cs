using LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.DeleteRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;
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
    [Authorize(Roles = "Manager")] // Giả định chỉ Admin được tạo
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateRoomMaintainScheduleCommand command)
    {
        var id = await mediator.Send(command);

        // Trả về 201 Created (Giả định bạn sẽ tạo endpoint 'GetById' sau)
        // Nếu chưa có GetById, bạn có thể tạm thời 'return Ok(id);'
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Get all room maintain schedules with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering schedules</param>
    /// <returns>List of schedules</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Manager")] // Giống LabRoomsController
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
    [Authorize(Roles = "Admin, Manager")] // Giả định Admin
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
    [Authorize(Roles = "Admin, Manager")] // Giả định Admin
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
        // Gán Id từ route vào command (giống LabRoomsController)
        command.Id = id;
        await mediator.Send(command);

        return NoContent(); // Trả về 204 No Content
    }

    /// <summary>
    /// Get a specific room maintain schedule by Id
    /// </summary>
    /// <param name="id">The Id of the schedule</param>
    /// <returns>The room maintain schedule</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Manager")] // Giả định Admin mới được xem
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
