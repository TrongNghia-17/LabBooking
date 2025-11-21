using LabBooking.Application.Features.Slots.Dtos;
using LabBooking.Application.Features.Slots.Queries.GetAllSlots;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController(
    IMediator mediator) : ControllerBase
    {        /// <summary>
        /// Get all slots
        /// </summary>
        /// <returns>List of slots</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SlotResponse>>> GetAll()
        {
            var query = new GetAllSlotsQuery();
            var slots = await mediator.Send(query);
            return Ok(slots);
        }
    }
}
