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
    public async Task<ActionResult<PagedResult<GetAllIncidentsResponse>>> GetAll([FromQuery] GetAllIncidentsQuery query)
    {
        var incidents = await cachingService.GetOrSetAsync(
            GetAllIncidentsCacheKey,
            () => mediator.Send(query),
            TimeSpan.FromMinutes(10)
            );

        return Ok(incidents);
    }
}
