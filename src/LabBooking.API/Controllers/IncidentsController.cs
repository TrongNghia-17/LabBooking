using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
using LabBooking.Application.Features.Incidents.Commands.Delete;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Báo cáo sự cố mới (Dành cho Student, Lecturer, Manager...).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SecurityGuard")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateIncidentCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Manager, SecurityGuard")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteIncidentCommand(id));
        return NoContent();
    }
}
