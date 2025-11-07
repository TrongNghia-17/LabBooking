using LabBooking.Application.Features.Equipments.Commands.DeleteEquipment;
using LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;

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

        return CreatedAtAction(nameof(GetById), new { id = newEquipmentId }, null);
    }

    /// <summary>
    /// Update an existing equipment
    /// </summary>
    /// <param name="id">The Id of the equipment to update</param>
    /// <param name="command">The new data for the equipment</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateEquipmentCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get a specific equipment by Id
    /// </summary>
    /// <param name="id">The Id of the equipment</param>
    /// <returns>The equipment</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EquipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EquipmentResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetEquipmentByIdQuery(id);
        var equipment = await mediator.Send(query);

        return Ok(equipment);
    }

    /// <summary>
    /// Get all equipments with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering equipments</param>
    /// <returns>List of equipments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EquipmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<EquipmentResponse>>> GetAll([FromQuery] GetAllEquipmentsQuery query)
    {
        var equipments = await mediator.Send(query);
        return Ok(equipments);
    }

    /// <summary>
    /// Delete an existing equipment
    /// </summary>
    /// <param name="id">The Id of the equipment to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteEquipmentCommand(id);
        await mediator.Send(command);

        return NoContent();
    }
}
