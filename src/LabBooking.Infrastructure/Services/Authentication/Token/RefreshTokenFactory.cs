namespace LabBooking.Infrastructure.Services.Authentication.Token;

public class RefreshTokenFactory(IJwtService jwtService) : IRefreshTokenFactory
{
    public RefreshToken Create(Guid userId)
    {
        var newRefreshTokenData = jwtService.GenerateRefreshToken();
        return new RefreshToken
        {
            Token = newRefreshTokenData.Token,
            Expires = newRefreshTokenData.Expires,
            UserId = userId
        };
    }
}
