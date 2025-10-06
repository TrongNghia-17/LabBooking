namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all incidents with optional filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering incidents</param>
    /// <returns>List of incidents</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllIncidentsResponse>>> GetAll([FromQuery] GetAllIncidentsQuery query)
    {
        var incidents = await mediator.Send(query);
        return Ok(incidents);
    }
}
