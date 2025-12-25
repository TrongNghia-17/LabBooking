using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.DeleteDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;
using LabBooking.Application.Features.DoorRequests.Commands.VerifyDoorAccess;
using LabBooking.Application.Features.DoorRequests.Dtos;
using LabBooking.Application.Features.DoorRequests.Queries.GetDailyManagerNotes;
using LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestDetail;
using LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestQr;
using LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequests;
using LabBooking.Domain.NonEntities;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoorRequestsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Tạo yêu cầu mở cửa cho một Booking (Chỉ chủ booking mới được tạo)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Lecturer, Student")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDoorRequestCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    /// <summary>
    /// Hủy yêu cầu mở cửa (Chỉ áp dụng cho yêu cầu đang chờ duyệt)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Lecturer, Student")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteDoorRequestCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách yêu cầu mở cửa (Đa năng).
    /// <para>- Nếu là Manager: Xem các yêu cầu gửi đến phòng Lab mình quản lý.</para>
    /// <para>- Nếu là Student/Lecturer: Xem danh sách yêu cầu do chính mình tạo.</para>
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<DoorRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<DoorRequestDto>>> GetAll([FromQuery] GetDoorRequestsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDoorRequestStatusDto requestBody)
    {
        var command = new UpdateDoorRequestStatusCommand(
            id,
            requestBody.NewStatus,
            requestBody.Note
        );

        await mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Xem chi tiết yêu cầu mở cửa.
    /// <para>
    /// API này tự động hiển thị thông tin liên hệ dựa trên người gọi:
    /// <br/>- <b>Manager:</b> Xem thông tin người gửi yêu cầu (SV/GV).
    /// <br/>- <b>Student/Lecturer:</b> Xem thông tin Manager quản lý phòng để liên hệ.
    /// </para>
    /// </summary>
    /// <param name="id">ID của Door Request</param>
    [HttpGet("{id}")]
    [Authorize] // Đã mở cho tất cả User đã đăng nhập
    [ProducesResponseType(typeof(DoorRequestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)] // Trả về nếu không phải Manager phòng đó hoặc chủ đơn
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoorRequestDetailDto>> GetById(Guid id)
    {
        var query = new GetDoorRequestDetailQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// [APP USER] Lấy thông tin vé để tạo mã QR.
    /// </summary>
    [HttpGet("{id}/qr-code")]
    [Authorize]
    [ProducesResponseType(typeof(DoorRequestQrDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoorRequestQrDto>> GetQrData(Guid id)
    {
        // Gửi Query sang Handler xử lý
        var result = await mediator.Send(new GetDoorRequestQrQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// [SECURITY GUARD] Kiểm tra mã QR (Verify).
    /// </summary>
    [HttpPost("verify-access")]
    [Authorize(Roles = "SecurityGuard, Manager, Admin")]
    [ProducesResponseType(typeof(VerifyAccessResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VerifyAccessResponse>> VerifyAccess([FromBody] VerifyAccessRequest requestBody)
    {
        // Gửi Command sang Handler xử lý
        // Lưu ý: Dùng requestBody.RequestId để tạo Command
        var result = await mediator.Send(new VerifyDoorAccessCommand(requestBody.RequestId));
        return Ok(result);
    }

    /// <summary>
    /// [SECURITY GUARD] Lấy danh sách ghi chú của Manager trong ngày (Dashboard bảo vệ).
    /// <para>Giúp bảo vệ biết trước ai sẽ đến, vào phòng nào và Manager dặn dò gì.</para>
    /// </summary>
    /// <param name="date">Ngày cần xem (Format: yyyy-MM-dd). Nếu để trống sẽ lấy ngày hôm nay.</param>
    [HttpGet("daily-notes")]
    // Chỉ Bảo vệ, Manager hoặc Admin mới được xem danh sách này
    [Authorize(Roles = "SecurityGuard")]
    [ProducesResponseType(typeof(List<DailyManagerNoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DailyManagerNoteDto>>> GetDailyManagerNotes([FromQuery] DateOnly? date)
    {
        // Gửi Query sang Handler xử lý
        var query = new GetDailyManagerNotesQuery(date);
        var result = await mediator.Send(query);

        return Ok(result);
    }
}
