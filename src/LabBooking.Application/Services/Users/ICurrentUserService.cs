namespace LabBooking.Application.Services.Users;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    IEnumerable<string> Roles { get; }
}
