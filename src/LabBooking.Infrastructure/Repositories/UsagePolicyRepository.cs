namespace LabBooking.Infrastructure.Repositories;

/// <summary>
/// Implements the repository for UsagePolicy data.
/// </summary>
internal class UsagePolicyRepository(LabBookingDbContext dbContext) : IUsagePolicyRepository
{
    public async Task<Guid> Create(UsagePolicy entity, CancellationToken cancellationToken = default)
    {
        dbContext.UsagePolicies.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
