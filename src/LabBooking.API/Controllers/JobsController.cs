using LabBooking.Application.Features.Jobs.AutoUpdateEquipmentStatus;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    [HttpPost("scan-equipment-status")]
    public async Task<IActionResult> RunScanJob()
    {
        if (!Request.Headers.TryGetValue("X-Job-Secret", out var secretKey))
        {
            return Unauthorized("Missing Secret Key");
        }

        var mySecret = configuration["JobSecretKey"];
        if (secretKey != mySecret)
        {
            return Unauthorized("Invalid Secret Key");
        }

        var result = await mediator.Send(new AutoUpdateEquipmentStatusJobCommand());
        return Ok(new { message = result, time = DateTime.UtcNow });
    }
}
