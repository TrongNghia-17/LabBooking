using LabBooking.Application.Features.UserDevices.Commands.RegisterDevice;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserDevicesController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
