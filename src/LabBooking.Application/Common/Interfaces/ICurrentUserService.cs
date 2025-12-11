namespace LabBooking.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    IEnumerable<string> Roles { get; }
}
