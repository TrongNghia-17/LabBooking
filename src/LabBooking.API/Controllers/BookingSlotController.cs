using LabBooking.Application.Features.BookingSlots.Dtos;
using LabBooking.Application.Features.BookingSlots.Queries.GetAllUnavailableSlot;
using LabBooking.Application.Features.Slots.Dtos;
using LabBooking.Application.Features.Slots.Queries.GetAllSlots;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingSlotController(
    IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingSlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BookingSlotResponse>>> GetAll([FromQuery] GetAllUnavailableSlotsQuery query)
        {
            var slots = await mediator.Send(query);
            return Ok(slots);
        }
    }
}
