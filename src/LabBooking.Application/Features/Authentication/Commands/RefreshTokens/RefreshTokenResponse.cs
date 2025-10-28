namespace LabBooking.Application.Features.Authentication.Commands.RefreshTokens;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiry);
