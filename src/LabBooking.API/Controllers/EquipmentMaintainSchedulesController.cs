using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAllEquipmentMaintainSchedules;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetEquipmentMaintainScheduleById;
using MediatR;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentMaintainSchedulesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new equipment maintain schedule
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")] // Giả định chỉ Admin được tạo
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentMaintainScheduleCommand command)
    {
        var id = await mediator.Send(command);

        // Trả về 201 Created (Giả định bạn sẽ tạo endpoint 'GetById' sau)
        //return CreatedAtAction(nameof(GetById), new { id }, null);
        return Ok(id);
    }

    /// <summary>
    /// Update an existing equipment maintain schedule
    /// </summary>
    /// <param name="id">The Id of the schedule to update</param>
    /// <param name="command">The new data for the schedule</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")] // Giả định Admin
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateEquipmentMaintainScheduleCommand command)
    {
        // Gán Id từ route vào command (giống LabRoomsController)
        command.Id = id;
        await mediator.Send(command);

        return NoContent(); // Trả về 204 No Content
    }

    /// <summary>
    /// Delete an existing equipment maintain schedule
    /// </summary>
    /// <param name="id">The Id of the schedule to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")] // Giả định Admin
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteEquipmentMaintainScheduleCommand(id);
        await mediator.Send(command);

        return NoContent(); // Trả về 204 No Content
    }

    /// <summary>
    /// Get a specific equipment maintain schedule by Id
    /// </summary>
    /// <param name="id">The Id of the schedule</param>
    /// <returns>The equipment maintain schedule</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")] // Giả định Admin mới được xem
    [ProducesResponseType(typeof(EquipmentMaintainScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EquipmentMaintainScheduleResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetEquipmentMaintainScheduleByIdQuery(id);
        var schedule = await mediator.Send(query);

        return Ok(schedule);
    }

    /// <summary>
    /// Get all equipment maintain schedules with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering schedules</param>
    /// <returns>List of schedules</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")] // Giống LabRoomsController
    [ProducesResponseType(typeof(PagedResult<EquipmentMaintainScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<EquipmentMaintainScheduleResponse>>> GetAll([FromQuery] GetAllEquipmentMaintainSchedulesQuery query)
    {
        var schedules = await mediator.Send(query);
        return Ok(schedules);
    }
}
