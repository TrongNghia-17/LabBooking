using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentMaintainScheduleController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(EquipmentMaintainBatchResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<EquipmentMaintainBatchResponse>> Create([FromBody] CreateEquipmentMaintainScheduleCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMaintainSchedulesQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
