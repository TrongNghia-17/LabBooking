using LabBooking.Application.Features.Notifications.Commands.SendNotificationToSelf;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("send-to-me")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendTestNotificationToSelf()
    {
        var command = new SendNotificationToSelfCommand();
        var result = await mediator.Send(command);

        return Ok(result);
    }
}
