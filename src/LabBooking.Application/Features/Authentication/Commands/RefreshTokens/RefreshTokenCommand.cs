namespace LabBooking.Application.Features.Authentication.Commands.RefreshTokens;

public record RefreshTokenCommand(string ExpiredRefreshToken) : IRequest<RefreshTokenResponse>;
