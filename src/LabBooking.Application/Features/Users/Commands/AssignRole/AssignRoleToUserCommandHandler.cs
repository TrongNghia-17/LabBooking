namespace LabBooking.Application.Features.Users.Commands.AssignRole;

public class AssignRoleToUserCommandHandler(
    ILogger<AssignRoleToUserCommandHandler> logger,
    UserManager<User> userManager
    ) : IRequestHandler<AssignRoleToUserCommand, Unit>
{
    public async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang THAY THẾ role thành {RoleName} cho user {UserId}", request.RoleName, request.UserId);

        var user = await userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new NotFoundException(nameof(User), request.UserId.ToString());

        // 1. Lấy danh sách tất cả các role hiện tại của user
        var currentRoles = await userManager.GetRolesAsync(user);

        // 2. Xóa tất cả các role cũ
        // Chỉ thực hiện xóa nếu user thực sự đang có role nào đó
        if (currentRoles.Any())
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                logger.LogError("Xóa các role cũ của user {UserId} thất bại", request.UserId);
                throw new Exception("Lỗi khi xóa các vai trò cũ của người dùng.");
            }
        }

        // 3. Thêm role mới
        var addResult = await userManager.AddToRoleAsync(user, request.RoleName);

        if (!addResult.Succeeded)
        {
            logger.LogError("Gán role mới {RoleName} cho user {UserId} thất bại", request.RoleName, request.UserId);
            throw new Exception(string.Join("\n", addResult.Errors.Select(e => e.Description)));
        }

        logger.LogInformation("Thay thế role thành công. User {UserId} bây giờ có role là {RoleName}", request.UserId, request.RoleName);
        return Unit.Value;
    }
}
