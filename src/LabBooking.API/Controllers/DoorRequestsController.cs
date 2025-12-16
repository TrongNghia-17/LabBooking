using LabBooking.Application.Features.DoorRequests.Commands.AcceptDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.CancelDoorRequest;
using LabBooking.Application.Features.DoorRequests.Commands.Create;
using LabBooking.Application.Features.DoorRequests.Queries.GetGuardPendingRequests;
using LabBooking.Application.Features.DoorRequests.Queries.GetHistory;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoorRequestsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Manager, Lecturer, Student")]
    public async Task<IActionResult> Create([FromBody] CreateDoorRequestCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(new { Id = id, Message = "Đã gửi yêu cầu cho bảo vệ!" });
    }

    [HttpGet("pending")]
    [Authorize(Roles = "SecurityGuard")]
    public async Task<IActionResult> GetPendingForGuard()
    {
        var result = await mediator.Send(new GetGuardPendingRequestsQuery());
        return Ok(result);
    }

    [HttpPost("accept/{id}")]
    [Authorize(Roles = "SecurityGuard")]
    public async Task<IActionResult> AcceptRequest(Guid id)
    {
        await mediator.Send(new AcceptDoorRequestCommand(id));
        return Ok(new { Message = "Đã nhận việc thành công! Hãy đi mở cửa ngay." });
    }

    [HttpGet("history")]
    [Authorize(Roles = "Manager, Lecturer, Student, SecurityGuard")]
    public async Task<IActionResult> GetHistory([FromQuery] GetDoorRequestHistoryQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("{id}")] // 1. Dùng HttpDelete
    [Authorize(Roles = "Lecturer, Student")]
    public async Task<IActionResult> DeleteRequest(Guid id)
    {
        // Lưu ý: Tên Command vẫn là CancelDoorRequestCommand cũng được, 
        // hoặc bạn có thể đổi tên class Command thành DeleteDoorRequestCommand cho đồng bộ tên gọi.
        await mediator.Send(new CancelDoorRequestCommand(id));

        // 2. Thông báo rõ là đã xóa
        return Ok(new { Message = "Đã xóa yêu cầu thành công." });
    }
}
