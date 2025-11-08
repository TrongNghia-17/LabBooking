namespace LabBooking.Application.Features.Authentication.Dtos;

/// <summary>
/// DTO containing authentication information returned to the client.
/// </summary>
/// <param name="AccessToken">The JWT Access Token.</param>
/// <param name="RefreshToken">The Refresh Token.</param>
/// <param name="RefreshTokenExpiry">The expiration time of the Refresh Token.</param>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiry
);
