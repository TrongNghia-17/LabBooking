using Google.Apis.Auth;

namespace LabBooking.Infrastructure.Authentication;

public class GoogleAuthService
{
    private readonly IConfiguration _config;

    public GoogleAuthService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<UserInfo?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var clientId = _config["Google:ClientId"];

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
            //var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            return new UserInfo
            {
                Email = payload.Email,
                Name = payload.Name,
                Picture = payload.Picture
            };
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
