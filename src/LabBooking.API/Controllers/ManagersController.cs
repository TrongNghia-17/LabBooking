using LabBooking.Application.Features.Managers.Dtos;
using LabBooking.Application.Features.Managers.Queries.GetManagerProfile;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Lấy thông tin profile của Manager và danh sách phòng Lab họ quản lý.
        /// </summary>
        [HttpGet("profile")]
        [Authorize(Roles = "Manager")] // Chỉ Manager mới gọi được api này
        [ProducesResponseType(typeof(ManagerProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ManagerProfileResponse>> GetManagerProfile()
        {
            var query = new GetManagerProfileQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
