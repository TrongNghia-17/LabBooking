using LabBooking.API.Filters;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.AutoUpdateStatus;

namespace LabBooking.API.Controllers;

[Route("api/internal-jobs")]
[ApiController]
[ApiKeyAuthorize]
public class InternalJobsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Endpoint được gọi bởi Render Cron Job để tự động cập nhật trạng thái bảo trì.
    /// </summary>
    [HttpPost("auto-update-maintenance-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Do ApiKeyAuthorize
    public async Task<IActionResult> RunAutoUpdateMaintenanceStatus()
    {
        var updatedCount = await mediator.Send(new AutoUpdateRoomMaintainStatusCommand());

        return Ok(new
        {
            Message = $"Cập nhật thành công {updatedCount} lịch bảo trì.",
            UpdatedCount = updatedCount
        });
    }
}
