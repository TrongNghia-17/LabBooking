using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.RefreshTokens;

/// <summary>
/// Command to refresh an expired Access Token using a valid Refresh Token.
/// </summary>
/// <param name="ExpiredRefreshToken">The refresh token string.</param>
public record RefreshTokenCommand(string ExpiredRefreshToken) : IRequest<AuthResponse>;
