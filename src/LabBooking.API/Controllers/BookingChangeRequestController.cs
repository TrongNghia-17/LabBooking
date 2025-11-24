using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.Booking.Queries.GetPendingBooking;
using LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest;
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
            var result = await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<BookingChangeRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingChangeRequestResponse>>> GetPending([FromQuery] Guid? labId)
        {
            var query = new GetPendingChangeRequestQuery(labId);
            var result = await mediator.Send(query);

            return Ok(result);
        }

    }
}
