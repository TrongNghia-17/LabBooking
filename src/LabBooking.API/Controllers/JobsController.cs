using LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController(IMediator mediator) : ControllerBase
{
    [HttpPost("update-maintenance-status")]
    public async Task<ActionResult> UpdateMaintenanceStatus()
    {
        var result = await mediator.Send(new AutoUpdateEquipmentStatusJobCommand());
        return Ok(result);
    }
}