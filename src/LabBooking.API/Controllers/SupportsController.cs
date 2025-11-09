using LabBooking.Application.Features.Supports.Commands.DeleteSupport;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupportsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new support ticket
    /// </summary>
    /// <param name="command">The data for the new support ticket</param>
    /// <returns>The newly created support ticket</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SupportsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateSupportCommand command)
    {
        var supportId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = supportId }, null);
    }

    /// <summary>
    /// Update an existing support ticket
    /// </summary>
    /// <param name="id">The Id of the support ticket to update</param>
    /// <param name="command">The new data for the support ticket</param> 
    /// <returns>No content (204) on successful update.</returns> 
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
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
    /// <returns>No content (204) on successful delete.</returns> 
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteSupportCommand(id);
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get all support tickets with optional filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering support tickets</param>
    /// <returns>A paged list of support tickets matching the query.</returns> 
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResult<SupportsResponse>), StatusCodes.Status200OK)]
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
    [Authorize(Roles = "Admin")]
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
