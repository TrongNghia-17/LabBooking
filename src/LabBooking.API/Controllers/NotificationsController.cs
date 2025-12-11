using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using LabBooking.Application.Features.Notifications.Commands.SendNotificationToSelf;
using LabBooking.Application.Features.Notifications.Dtos;
using LabBooking.Application.Features.Notifications.Queries.GetAllNotifications;

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

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NotificationsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<NotificationsResponse>>> GetAllNotifications(
      [FromQuery] GetAllNotificationsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var command = new MarkNotificationAsReadCommand(id);

        await mediator.Send(command);

        return NoContent();
    }

}
