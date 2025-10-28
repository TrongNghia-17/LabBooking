namespace LabBooking.Application.Services.Authentication.Token;

public interface IRefreshTokenFactory
{
    RefreshToken Create(Guid userId);
}
