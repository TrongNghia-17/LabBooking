namespace LabBooking.Domain.Repositories;

public interface IUserDeviceRepository
{
    Task<UserDevice?> GetByTokenAsync(string token, CancellationToken cancellation = default);
    Task AddAsync(UserDevice device, CancellationToken cancellation = default);
    Task Update(UserDevice device, CancellationToken cancellation = default);
    Task<List<string>> GetTokensByUserIdAsync(Guid userId, CancellationToken cancellation = default);
}
