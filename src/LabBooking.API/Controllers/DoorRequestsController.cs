using LabBooking.Application.Services.Caching;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoorRequestsController(
    IMediator mediator,
    ICachingService cachingService) : ControllerBase
{
    private const string GetAllDoorRequestsCacheKey = "get_all_door_requests";

    /// <summary>
    /// Get all door requests with optional filtering
    /// </summary>
    /// <param name="query">Query parameters for filtering door requests</param>
    /// <returns>Paged list of door requests</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<DoorRequestsResponse>>> GetAll(
        [FromQuery] GetAllDoorRequestsQuery query)
    {
        //var doorRequests = await cachingService.GetOrSetAsync(
        //    GetAllDoorRequestsCacheKey,
        //    () => mediator.Send(query),
        //    TimeSpan.FromMinutes(10));

        var doorRequests = await mediator.Send(query);

        return Ok(doorRequests);
    }

    [HttpDelete("clear-all-doorrequest-cache")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ClearAllDoorRequestsCache()
    {
        await cachingService.RemoveAsync(GetAllDoorRequestsCacheKey);
        return Ok($"Cache with key '{GetAllDoorRequestsCacheKey}' has been cleared.");
    }
}
