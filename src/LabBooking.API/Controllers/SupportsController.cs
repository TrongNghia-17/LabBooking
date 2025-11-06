using LabBooking.Application.Features.Supports.Commands.DeleteSupport;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupportsController(
    IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new support ticket
    /// </summary>
    /// <param name="command">The data for the new support ticket</param>
    /// <returns>The newly created support ticket</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SupportsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportsResponse>> Create([FromBody] CreateSupportCommand command)
    {
        var supportResponse = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = supportResponse.Id }, supportResponse);
    }

    /// <summary>
    /// Update an existing support ticket
    /// </summary>
    /// <param name="id">The Id of the support ticket to update</param>
    /// <param name="dto">The new data for the support ticket</param>
    /// <returns>The updated support ticket</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SupportsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportsResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateSupportCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing support ticket
    /// </summary>
    /// <param name="id">The Id of the support ticket to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteSupportCommand(id);
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get all support tickets with optional filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering support tickets</param>
    /// <returns>List of support tickets</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<SupportsResponse>>> GetAll([FromQuery] GetAllSupportsQuery query)
    {
        var supports = await mediator.Send(query);
        return Ok(supports);
    }

    /// <summary>
    /// Get a specific support ticket by Id
    /// </summary>
    /// <param name="id">The Id of the support ticket</param>
    /// <returns>The support ticket</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupportsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupportsResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetSupportByIdQuery(id);
        var support = await mediator.Send(query);

        return Ok(support);
    }
}
