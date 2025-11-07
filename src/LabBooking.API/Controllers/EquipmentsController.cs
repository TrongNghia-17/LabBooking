using LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentsController(
    IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new equipment
    /// </summary>
    /// <param name="command">The data for the new equipment</param>
    /// <returns>The newly created equipment</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Create([FromBody] CreateEquipmentCommand command)
    {
        var newEquipmentId = await mediator.Send(command);

        // return CreatedAtAction(nameof(GetById), new { id = newEquipmentId }, null);

        return Created(string.Empty, new { Id = newEquipmentId });
    }
}
