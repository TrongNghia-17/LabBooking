using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.Booking.Queries.GetPendingBooking;
using LabBooking.Application.Features.BookingChangeRequest.Commands.ApproveBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Commands.RejectBookingChangeRequest;
using LabBooking.Application.Features.BookingChangeRequest.Dtos;
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

        [HttpPut("reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectChangeRequest([FromBody] RejectBookingChangeRequestCommand command)
        {
            try
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
            catch (InvalidOperationException ex) // Bắt đúng loại lỗi bạn ném
            {
                // Biến lỗi 500 thành 400 và lấy message ra trả về
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Các lỗi khác không ngờ tới thì mới để 500
                return StatusCode(500, new { message = "Lỗi hệ thống không xác định." });
            }
            
        }

        [HttpPut("approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveChangeRequest([FromBody] ApproveBookingChangeRequestCommand command)
        {
            try
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
            catch (InvalidOperationException ex) // Bắt đúng loại lỗi bạn ném
            {
                // Biến lỗi 500 thành 400 và lấy message ra trả về
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Các lỗi khác không ngờ tới thì mới để 500
                return StatusCode(500, new { message = "Lỗi hệ thống không xác định." });
            }
        }
    }
}
