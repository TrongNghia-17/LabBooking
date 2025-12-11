namespace LabBooking.Application.Interfaces.Authentication.Token;

public interface IRefreshTokenFactory
{
    RefreshToken Create(Guid userId);
}
