namespace LabBooking.Application.Services.Authentication.External;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken);
}