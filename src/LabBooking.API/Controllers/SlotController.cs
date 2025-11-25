using LabBooking.Application.Features.Slots.Commands.CreateSlot;
using LabBooking.Application.Features.Slots.Commands.DeleteSlot;
using LabBooking.Application.Features.Slots.Commands.UpdateSlot;
using LabBooking.Application.Features.Slots.Dtos;
using LabBooking.Application.Features.Slots.Queries.GetAllSlots;
using LabBooking.Application.Features.Slots.Queries.GetSlotById;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController(IMediator mediator) : ControllerBase
    {       /// <summary>
            /// Get all slots
            /// </summary>
            /// <returns>List of slots</returns>
        [HttpGet]
        //[Authorize(Roles = "Admin, Manager, User")]
        [ProducesResponseType(typeof(IEnumerable<SlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SlotResponse>>> GetAll()
        {
            var query = new GetAllSlotsQuery();
            var slots = await mediator.Send(query);
            return Ok(slots);
        }

        /// <summary>
        /// Create a new slot configuration
        /// </summary>
        /// <param name="command">The data for the new slot</param>
        /// <returns>The newly created slot id</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được tạo Slot
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateSlotCommand command)
        {
            var id = await mediator.Send(command);

            // Lưu ý: Bạn cần implement GetById cho Slot để dùng CreatedAtAction chính xác
            // Tạm thời return Ok(id) hoặc Created
            return CreatedAtAction(nameof(Create), new { id }, id);
        }

        /// <summary>
        /// Cập nhật thông tin một Slot
        /// </summary>
        /// <param name="id">ID của Slot cần sửa</param>
        /// <param name="command">Thông tin cần cập nhật</param>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSlotCommand command)
        {
            if (id != command.Id && command.Id != Guid.Empty)
            {
                return BadRequest("ID trong URL không khớp với ID trong body.");
            }

            command.Id = id; // Đảm bảo ID chính xác
            await mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một Slot theo ID
        /// </summary>
        /// <param name="id">ID của Slot</param>
        /// <returns>Thông tin Slot</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin, Manager, User")] // Ai cũng có thể xem chi tiết Slot
        [ProducesResponseType(typeof(SlotResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SlotResponse>> GetById([FromRoute] Guid id)
        {
            try
            {
                var response = await mediator.Send(new GetSlotByIdQuery(id));
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy Slot yêu cầu.");
            }
        }

        /// <summary>
        /// Xóa một Slot khỏi hệ thống
        /// </summary>
        /// <param name="id">ID của Slot cần xóa</param>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được xóa Slot
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await mediator.Send(new DeleteSlotCommand(id));
                return NoContent(); // Trả về 204 khi xóa thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
