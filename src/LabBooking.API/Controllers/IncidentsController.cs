using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
using LabBooking.Application.Features.Incidents.Commands.Delete;
using LabBooking.Application.Features.Incidents.Dtos;
using LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;
using LabBooking.Application.Features.Incidents.Queries.GetStatistics;
using LabBooking.Domain.Constants;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Báo cáo sự cố mới (Dành cho Bảo vệ từ phiếu Check, hoặc các vai trò khác báo cáo độc lập).
    /// </summary>
    [HttpPost]
    // THAY ĐỔI: Mở rộng quyền cho nhiều vai trò hơn
    [Authorize(Roles = "SecurityGuard, Manager, Lecturer, Student")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // ...
    public async Task<IActionResult> Create([FromBody] CreateIncidentCommand command)
    {
        var id = await mediator.Send(command);
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
    [Authorize(Roles = "Manager, SecurityGuard")]
    [ProducesResponseType(typeof(PagedResult<IncidentResponse>), StatusCodes.Status200OK)] // Nhớ thay IncidentDto đúng tên của bạn
    public async Task<IActionResult> GetAll([FromQuery] GetIncidentsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("statistics")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")] // Chỉ Admin và Manager được xem
    public async Task<IActionResult> GetStatistics([FromQuery] int year)
    {
        if (year == 0)
        {
            year = DateTime.UtcNow.Year; // Mặc định lấy năm hiện tại nếu không cung cấp
        }

        var query = new GetIncidentStatisticsQuery { Year = year };
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
