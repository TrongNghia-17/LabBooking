using LabBooking.Application.Features.Users.Dtos;
using LabBooking.Application.Features.Users.Queries.GetAllUsers;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy tất cả người dùng (Hỗ trợ Lọc, Sắp xếp, Phân trang và Lọc theo Role)
    /// </summary>
    /// <returns>Danh sách người dùng đã phân trang</returns>
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAll(
        [FromQuery] GetAllUsersQuery query
    )
    {
        var users = await mediator.Send(query);
        return Ok(users);
    }
}
