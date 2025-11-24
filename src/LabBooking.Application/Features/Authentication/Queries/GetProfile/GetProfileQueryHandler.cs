using LabBooking.Application.Features.Authentication.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Authentication.Queries.GetProfile;

/// <summary>
/// Handles the GetProfileQuery to retrieve the authenticated user's profile.
/// </summary>
public class GetProfileQueryHandler(
    ILogger<GetProfileQueryHandler> logger,
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IMapper mapper
) : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        // 1. Get the User ID from the token (via ICurrentUserService)
        var userId = currentUserService.UserId;

        if (userId == null)
        {
            logger.LogWarning("GetProfileQuery: Could not find UserId in ICurrentUserService.");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        // 2. Find the user in the database
        var user = await userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            logger.LogWarning("User not found with ID: {UserId}", userId.Value);
            throw new NotFoundException(nameof(User), userId.Value.ToString());
        }

        // 3. Map basic fields from User entity to UserProfileResponse DTO
        var userProfile = mapper.Map<UserProfileResponse>(user);

        // 4. Get the user's roles (asynchronous call)
        var roles = await userManager.GetRolesAsync(user);

        // 5. Return the complete profile DTO, now including the roles
        // We use 'with' (record feature) to create a copy with the Roles property updated.
        return userProfile with { Roles = roles };
    }
}
