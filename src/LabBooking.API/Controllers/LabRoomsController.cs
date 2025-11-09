using LabBooking.Application.Features.LabRooms.Commands.DeleteLabRoom;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LabRoomsController(
    IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new lab room
    /// </summary>
    /// <param name="command">The data for the new lab room</param>
    /// <returns>The newly created lab room</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateLabRoomCommand command)
    {
        var id = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Update an existing lab room
    /// </summary>
    /// <param name="id">The Id of the lab room to update</param>
    /// <param name="command">The new data for the lab room</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLabRoomCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing lab room
    /// </summary>
    /// <param name="id">The Id of the lab room to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteLabRoomCommand(id);
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get all lab rooms with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering lab rooms</param>
    /// <returns>List of lab rooms</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResult<LabRoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<LabRoomResponse>>> GetAll([FromQuery] GetAllLabRoomsQuery query)
    {
        var labRooms = await mediator.Send(query);
        return Ok(labRooms);
    }

    /// <summary>
    /// Get a specific lab room by Id
    /// </summary>
    /// <param name="id">The Id of the lab room</param>
    /// <returns>The lab room</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LabRoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LabRoomResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetLabRoomByIdQuery(id);
        var labRoom = await mediator.Send(query);

        return Ok(labRoom);
    }
}
