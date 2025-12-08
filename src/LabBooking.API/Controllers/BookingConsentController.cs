using LabBooking.Application.Features.BookingConsentRequest.Commands.ResolveBookingConsent;
using LabBooking.Application.Features.BookingConsentRequest.Queries.GetRescheduleBookingConsent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingConsentController(IMediator mediator) : ControllerBase
    {
        // POST: api/BookingConsent/resolve
        [HttpPost("resolve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResolveConsent([FromBody] ResolveBookingConsentCommand command)
        {
            // Gọi qua MediatR -> Vào Handler -> Vào Repository
            var result = await mediator.Send(command);
            return Ok(new { success = result });
        }

        // GET: api/BookingConsent/{id}/reschedule-context
        [HttpGet("{id}/reschedule-context")]
        public async Task<IActionResult> GetRescheduleContext(Guid id)
        {
            var query = new GetRescheduleBookingConsentQuery(id);
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
