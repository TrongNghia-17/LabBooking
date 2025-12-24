namespace LabBooking.Application.Features.Authentication.Dtos;

/// <summary>
/// DTO containing the user's profile information.
/// </summary>
public record UserProfileResponse
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public IList<string> Roles { get; init; } = [];
}
