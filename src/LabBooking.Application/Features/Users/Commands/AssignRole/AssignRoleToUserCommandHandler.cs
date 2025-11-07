namespace LabBooking.Application.Features.Users.Commands.AssignRole;

public class AssignRoleToUserCommandHandler(
    ILogger<AssignRoleToUserCommandHandler> logger,
    UserManager<User> userManager
    ) : IRequestHandler<AssignRoleToUserCommand, Unit>
{
    public async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang gán role {RoleName} cho user {UserId}", request.RoleName, request.UserId);

        var user = await userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new NotFoundException(nameof(User), request.UserId.ToString());

        if (await userManager.IsInRoleAsync(user, request.RoleName))
        {
            logger.LogWarning("User {UserId} đã có role {RoleName}", request.UserId, request.RoleName);
            return Unit.Value;
        }

        var result = await userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            logger.LogError("Gán role {RoleName} cho user {UserId} thất bại", request.RoleName, request.UserId);
            throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
        }

        logger.LogInformation("Gán role thành công");
        return Unit.Value;
    }
}
