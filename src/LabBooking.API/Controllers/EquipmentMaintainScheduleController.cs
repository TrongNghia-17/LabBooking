using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

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
}
