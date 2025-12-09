namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy thông tin phòng Lab và thiết bị chưa bảo trì thuộc quyền quản lý của Manager hiện tại.
    /// </summary>
    [HttpGet("lab-details")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ManagerLabDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ManagerLabDetailsResponse>> GetManagerLabDetails()
    {
        var query = new GetManagerLabDetailsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
