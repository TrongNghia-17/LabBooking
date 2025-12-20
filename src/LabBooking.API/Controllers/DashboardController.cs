using LabBooking.Application.Features.Dashboard.Dtos;
using LabBooking.Application.Features.Dashboard.Queries.GetSystemHealthDashboardQuery;
using LabBooking.Domain.Constants;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")] // Chỉ Admin và Manager được xem
public class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy dữ liệu thống kê tổng quan về sức khỏe hệ thống.
    /// </summary>
    [HttpGet("system-health")]
    [ProducesResponseType(typeof(SystemHealthDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemHealthDashboard()
    {
        var query = new GetSystemHealthDashboardQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}