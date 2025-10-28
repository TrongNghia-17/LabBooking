namespace LabBooking.Infrastructure.Services.Authentication.External;

public class GoogleAuthService(
    IConfiguration config,
    ILogger<GoogleAuthService> logger
    ) : IGoogleAuthService
{
    public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var clientId = config["Google:ClientId"];

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [clientId]
                });

            return payload;
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "Invalid Google JWT received. Token: {IdToken}", idToken);
            return null;
        }
    }
}
