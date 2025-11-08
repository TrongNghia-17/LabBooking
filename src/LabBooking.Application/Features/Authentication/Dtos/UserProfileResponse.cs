namespace LabBooking.Application.Features.Authentication.Dtos;

/// <summary>
/// DTO containing the user's profile information.
/// </summary>
public record UserProfileResponse
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string? UserName { get; init; }
    public IList<string> Roles { get; init; } = [];
}
