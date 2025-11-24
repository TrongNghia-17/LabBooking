using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Handles the LogoutUserCommand to sign out the current user.
/// </summary>
public class LogoutUserCommandHandler(
    ILogger<LogoutUserCommandHandler> logger,
    UserManager<User> userManager,
    ICurrentUserService currentUserService
) : IRequestHandler<LogoutUserCommand, Unit>
{
    public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the User ID from the token (via ICurrentUserService)
        var userId = currentUserService.UserId;

        if (userId == null)
        {
            logger.LogWarning("An unauthenticated user attempted to log out.");
            return Unit.Value;
        }

        // 2. Find the user
        var user = await userManager.FindByIdAsync(userId.Value.ToString())
            ?? throw new NotFoundException(nameof(User), userId.Value.ToString());

        // 3. Update the Security Stamp.
        var result = await userManager.UpdateSecurityStampAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError("Failed to update Security Stamp for user {UserId}: {Errors}", userId, errors);
            throw new Exception("Could not complete logout process.");
        }

        logger.LogInformation("User {UserId} logged out successfully (Security Stamp updated).", userId);
        return Unit.Value;
    }
}
