using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.Booking.Queries.GetPendingBooking;
using LabBooking.Application.Features.BookingChangeRequest.Commands.ApproveBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Commands.RejectBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using LabBooking.Application.Features.BookingChangeRequest.Queries.GetBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Queries.GetPendingBookingChangeRequest;
using LabBooking.Application.Features.Bookings.Commands.CreateBooking;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingChangeRequestController(
        IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBookingChangeRequest([FromBody] CreateBookingChangeRequestCommand command)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                command = command with { RequestedById = userId };
            }
            var result = await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<BookingChangeRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<BookingChangeRequestResponse>>> GetPending()
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }
            var query = new GetPendingChangeRequestQuery(userId);
            var result = await mediator.Send(query);

            return Ok(result);
        }

        [HttpGet] // Hoặc [HttpGet("all")] nếu muốn rõ ràng đường dẫn
        [ProducesResponseType(typeof(List<BookingChangeRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Student, Lecturer")]
        public async Task<ActionResult<List<BookingChangeRequestResponse>>> GetAll()
        {
            // Lấy UserID từ Token
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            // Tạo Query
            var query = new GetAllChangeRequestsQuery(userId);

            // Gửi qua Mediator
            var result = await mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectChangeRequest([FromBody] RejectBookingChangeRequestCommand command)
        {
            // Tự lấy ManagerId từ Token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdString, out var managerId))
            {
                command = command with { ManagerId = managerId };
            }

            await mediator.Send(command);
            return Ok(new { message = "Đã từ chối yêu cầu thay đổi." });
        }

        [HttpPut("approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveChangeRequest([FromBody] ApproveBookingChangeRequestCommand command)
        {
            // Lấy ID Manager từ Token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdString, out var managerId))
            {
                // Override ManagerId từ Token để bảo mật (tránh FE gửi bậy)
                command = command with { ManagerId = managerId };
            }

            // Gửi lệnh
            // Lưu ý: Frontend gửi body { "bookingId": "..." } nhưng thực chất đó là ID của Request
            // Bạn nên chắc chắn DTO map đúng, hoặc đổi tên DTO FE gửi lên cho khớp

            await mediator.Send(command);

            return Ok(new { message = "Đã duyệt yêu cầu thay đổi." });
        }
    }
}
