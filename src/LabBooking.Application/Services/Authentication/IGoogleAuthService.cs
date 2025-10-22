
namespace LabBooking.Infrastructure.Services.Authentication
{
    public interface IGoogleAuthService
    {
        Task<UserInfo?> VerifyGoogleTokenAsync(string idToken);
    }
}