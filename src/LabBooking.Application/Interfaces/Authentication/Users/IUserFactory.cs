namespace LabBooking.Application.Services.Authentication.Users;

public interface IUserFactory
{
    User CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payload);
}
