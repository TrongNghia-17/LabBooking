using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
using LabBooking.Application.Features.Incidents.Commands.Delete;
using LabBooking.Application.Features.Incidents.Dtos;
using LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Báo cáo sự cố mới (Dành cho Bảo vệ tạo từ phiếu Check hoặc báo lẻ).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SecurityGuard")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateIncidentCommand command)
    {
        var id = await mediator.Send(command);

        // FIX: Trả về 201 Created kèm ID (An toàn nhất khi chưa có API GetById)
        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    /// <summary>
    /// Xóa báo cáo sự cố (Soft Delete).
    /// <para>Nếu sự cố là hỏng thiết bị, thiết bị sẽ tự động được mở khóa (Available).</para>
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager, SecurityGuard")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Trả về nếu thiết bị đang bảo trì
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteIncidentCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách sự cố (Có filter).
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<IncidentResponse>), StatusCodes.Status200OK)] // Nhớ thay IncidentDto đúng tên của bạn
    public async Task<IActionResult> GetAll([FromQuery] GetIncidentsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
