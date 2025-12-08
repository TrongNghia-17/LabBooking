namespace LabBooking.Infrastructure.Repositories;

internal class UserDeviceRepository(LabBookingDbContext dbContext) : IUserDeviceRepository
{
    public async Task AddAsync(UserDevice device, CancellationToken cancellation = default)
    {
        await dbContext.UserDevices.AddAsync(device, cancellation);
        await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<UserDevice?> GetByTokenAsync(string token, CancellationToken cancellation = default)
    {
        return await dbContext.UserDevices
            .FirstOrDefaultAsync(d => d.PushToken == token, cancellation);
    }

    public async Task Update(UserDevice device, CancellationToken cancellation = default)
    {
        dbContext.UserDevices.Update(device);
        await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<List<string>> GetTokensByUserIdAsync(Guid? userId, CancellationToken cancellation = default)
    {
        return await dbContext.UserDevices
            .Where(d => d.UserId == userId)
            .Select(d => d.PushToken)
            .ToListAsync(cancellation);
    }
}
