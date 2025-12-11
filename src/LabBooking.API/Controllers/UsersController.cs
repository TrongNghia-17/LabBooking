using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Users.Commands.AssignRole;
using LabBooking.Application.Features.Users.Dtos;
using LabBooking.Application.Features.Users.Queries.GetAllUsers;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy tất cả người dùng (Hỗ trợ Lọc, Sắp xếp, Phân trang và Lọc theo Role)
    /// </summary>
    /// <returns>Danh sách người dùng đã phân trang</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Gán một role cho người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng (từ route)</param>
    /// <param name="command">Thông tin role (chỉ cần RoleName trong body)</param>
    /// <returns>No content</returns>
    [HttpPost("{userId:guid}/roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AssignRole(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleToUserCommand command)
    {
        command.UserId = userId;
        await mediator.Send(command);
        return NoContent();
    }
}
