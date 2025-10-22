using Google.Apis.Auth;

namespace LabBooking.Application.Services.Authentication;

public interface IUserFactory
{
    User CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payload);
}
