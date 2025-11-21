using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest;
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
    }
}
