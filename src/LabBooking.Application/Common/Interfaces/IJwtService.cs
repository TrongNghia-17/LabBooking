namespace LabBooking.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string email, string name);
}