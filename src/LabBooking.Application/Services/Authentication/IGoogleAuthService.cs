using Google.Apis.Auth;
using LabBooking.Application.Dtos.Authentication;

namespace LabBooking.Application.Services.Authentication;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken);
}