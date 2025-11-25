using LabBooking.Application.Features.ApproveBooking.Commands;
using LabBooking.Application.Features.ApproveBooking.Dtos;
using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.Booking.Queries.GetBookingById;
using LabBooking.Application.Features.Booking.Queries.GetChangeableBooking;
using LabBooking.Application.Features.Booking.Queries.GetPendingBooking;
using LabBooking.Application.Features.Bookings.Commands.CreateBooking;
using LabBooking.Application.Features.Equipments.Queries.GetAllEquipments;
using LabBooking.Application.Features.HistoryBooking.Queries.GetMyBookingHistory;
using LabBooking.Application.Features.Slots.Dtos;
using LabBooking.Application.Features.Slots.Queries.GetAllSlots;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(
    IMediator mediator,
    ICachingService cachingService) : ControllerBase
{

    /// <summary>
    /// Creates a new booking (Teaching, Project, or Priority).
    /// </summary>
    /// <param name="command">The command containing booking details, slots, and type-specific info.</param>
    /// <returns>The newly created booking.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        var result = await mediator.Send(command);

        // Xóa cache danh sách booking để người dùng thấy dữ liệu mới ngay
        //await cachingService.RemoveAsync(GetAllBookingsCacheKey);

        // LƯU Ý: Bạn cũng nên xóa cache của API "GetUnavailableSlots" nếu có cache API đó,
        // vì booking mới sẽ làm thay đổi các slot trống.
        // await cachingService.RemoveAsync("unavailable_slots_cache_key"); 

        return StatusCode(StatusCodes.Status201Created, result);
    }

    //[HttpDelete("clear-all-bookings-cache")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //public async Task<IActionResult> ClearAllBookingsCache()
    //{
    //    await cachingService.RemoveAsync(GetAllBookingsCacheKey);
    //    return Ok($"Cache with key '{GetAllBookingsCacheKey}' has been cleared.");
    //}

    /// <summary>
    /// Lấy danh sách các Booking đã duyệt và còn hạn đổi lịch của User.
    /// </summary>
    /// <param name="userId">ID của User (Frontend truyền lên hoặc lấy từ Token)</param>
    [HttpGet("changeable")] // Route sẽ là: GET /api/Bookings/changeable?userId=...
    [ProducesResponseType(typeof(List<BookingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BookingResponse>>> GetChangeableBookings()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var query = new GetChangeableBookingsQuery(userId);
        // 2. Gửi đi
        var bookings = await mediator.Send(query);

        // 3. Trả về kết quả
        return Ok(bookings);
    }

    // GET: api/Bookings/5f8d...
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id) // Nhận Guid id từ URL
    {
        // Tạo Query object từ id nhận được
        var query = new GetBookingByIdQuery(id);

        // Gửi sang Mediator xử lý
        var result = await mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách các Booking đang chờ duyệt (Pending).
    /// Dành cho Manager/Admin.
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<BookingResponse>>> GetPending([FromQuery] Guid? labId)
    {
        var query = new GetPendingBookingsQuery(labId);
        var result = await mediator.Send(query);
        return Ok(result);
    }


    [HttpPut("approve")]
    [ProducesResponseType(typeof(ApproveBookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingCommand command)
    {
        // Command chứa { BookingId, ManagerId }
        await mediator.Send(command);
        return Ok(new { message = "Duyệt đơn thành công." });
    }

    [HttpGet("my-history-booking")]
    //[Authorize]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<BookingResponse>>> GetMyHistory()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var query = new GetMyBookingHistoryQuery(userId);
        var result = await mediator.Send(query);

        return Ok(result);
    }
}