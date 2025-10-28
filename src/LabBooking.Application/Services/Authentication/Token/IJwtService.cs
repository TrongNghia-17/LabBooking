namespace LabBooking.Application.Services.Authentication.Token;

public record RefreshTokenData(string Token, DateTime Expires);

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    RefreshTokenData GenerateRefreshToken();
}