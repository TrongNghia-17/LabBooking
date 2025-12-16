using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.LabRooms.Commands.DeleteLabRoom;
using LabBooking.Application.Features.LabRooms.Queries.GetAvailableLabsByDate;
using LabBooking.Application.Features.LabRooms.Queries.GetLabStatistics;
using LabBooking.Application.Features.LabRooms.Queries.GetTopLabs;
using LabBooking.Application.Features.LabRooms.Queries.GetUnmaintainedLabRooms;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class LabRoomsController(
    IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new lab room
    /// </summary>
    /// <param name="command">The data for the new lab room</param>
    /// <returns>The newly created lab room</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateLabRoomCommand command)
    {
        var id = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Update an existing lab room
    /// </summary>
    /// <param name="id">The Id of the lab room to update</param>
    /// <param name="command">The new data for the lab room</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLabRoomCommand command)
    {
        command.Id = id;
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing lab room
    /// </summary>
    /// <param name="id">The Id of the lab room to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteLabRoomCommand(id);
        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Get all lab rooms with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering lab rooms</param>
    /// <returns>List of lab rooms</returns>
    [HttpGet]
    [Authorize(Roles = "Admin, Student, Lecturer, Manager, SecurityGuard")]
    [ProducesResponseType(typeof(PagedResult<LabRoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<LabRoomResponse>>> GetAll([FromQuery] GetAllLabRoomsQuery query)
    {
        var labRooms = await mediator.Send(query);
        return Ok(labRooms);
    }

    /// <summary>
    /// Get all lab rooms that have a pending maintenance schedule (NotYet).
    /// </summary>
    /// <returns>A list of lab rooms with pending maintenance.</returns>
    [HttpGet("unmaintained")]
    [Authorize(Roles = "Admin, Manager")] // Chỉ dành cho người quản lý/Admin xem
    [ProducesResponseType(typeof(IEnumerable<LabRoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<LabRoomResponse>>> GetUnmaintained()
    {
        var query = new GetUnmaintainedLabRoomsQuery();
        var labRooms = await mediator.Send(query);
        return Ok(labRooms);
    }

    /// <summary>
    /// Get a specific lab room by Id
    /// </summary>
    /// <param name="id">The Id of the lab room</param>
    /// <returns>The lab room</returns>
    [HttpGet("{id:guid}")]
    //[Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LabRoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LabRoomResponse>> GetById([FromRoute] Guid id)
    {
        var query = new GetLabRoomByIdQuery(id);
        var labRoom = await mediator.Send(query);

        return Ok(labRoom);
    }

    /// <summary>
    /// Thống kê phòng Lab được đặt nhiều nhất theo từng tháng trong năm.
    /// </summary>
    /// <param name="year">Năm cần xem (VD: 2024). Bỏ trống sẽ lấy năm nay.</param>
    [HttpGet("top-labs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTopLabs([FromQuery] int year)
    {
        var result = await mediator.Send(new GetTopLabsQuery(year));
        return Ok(result);
    }

    [HttpGet("labs")]
    [Authorize(Roles = "Admin")] // Bật lại nếu cần bảo mật
    public async Task<IActionResult> GetLabStatistics([FromQuery] int year)
    {
        // Nếu không truyền năm, lấy năm hiện tại
        if (year <= 0) year = DateTime.Now.Year;

        var result = await mediator.Send(new GetLabStatisticsQuery(year));
        return Ok(result);
    }

    /// <summary>
    /// API cho Coze/Frontend kiểm tra phòng trống theo ngày
    /// </summary>
    /// <param name="date">Định dạng yyyy-MM-dd</param>
    [HttpGet("check-availability")]
    [AllowAnonymous] // Mở public để Coze gọi được (nếu dùng Ngrok free)
    [ProducesResponseType(typeof(IEnumerable<LabRoomAvailabilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability([FromQuery] DateOnly date)
    {
        // ASP.NET Core tự động parse chuỗi "2025-12-20" thành DateOnly
        var query = new GetAvailableLabsByDateQuery(date);

        var result = await mediator.Send(query);

        return Ok(result);
    }
}
