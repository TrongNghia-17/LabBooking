using LabBooking.Application.Services.Caching;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController(
    IMediator mediator,
    ICachingService cachingService) : ControllerBase
{
    private const string GetAllIncidentsCacheKey = "get_all_incidents";

    /// <summary>
    /// Get all incidents with optional filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering incidents</param>
    /// <returns>List of incidents</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<IncidentsResponse>>> GetAll([FromQuery] GetAllIncidentsQuery query)
    {
        var incidents = await cachingService.GetOrSetAsync(
            GetAllIncidentsCacheKey,
            () => mediator.Send(query),
            TimeSpan.FromMinutes(10)
            );

        return Ok(incidents);
    }

    /// <summary>
    /// Creates a new incident.
    /// </summary>
    /// <param name="command">The command containing the data for the new incident.</param>
    /// <returns>The newly created incident.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateLab([FromBody] CreateIncidentCommand command)
    {
        var incident = await mediator.Send(command);
        await cachingService.RemoveAsync(GetAllIncidentsCacheKey);

        return CreatedAtAction(null, new { id = incident.Id }, incident);
    }

    [HttpDelete("clear-all-incidents-cache")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ClearAllIncidentsCache()
    {
        await cachingService.RemoveAsync(GetAllIncidentsCacheKey);
        return Ok($"Cache with key '{GetAllIncidentsCacheKey}' has been cleared.");
    }
}
