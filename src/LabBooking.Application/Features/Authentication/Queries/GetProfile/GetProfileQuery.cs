using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Queries.GetProfile;

/// <summary>
/// Query to get the profile information of the currently logged-in user.
/// No parameters are needed as the User ID will be retrieved from ICurrentUserService.
/// </summary>
public record GetProfileQuery() : IRequest<UserProfileResponse>;
