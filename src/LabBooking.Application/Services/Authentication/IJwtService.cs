namespace LabBooking.Application.Services.Authentication;

public interface IJwtService
{
    string GenerateToken(string email, string name);
}