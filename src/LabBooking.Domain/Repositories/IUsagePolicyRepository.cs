namespace LabBooking.Domain.Repositories;

/// <summary>
/// Interface for the repository handling UsagePolicy data.
/// </summary>
public interface IUsagePolicyRepository
{
    /// <summary>
    /// Creates a new usage policy in the database.
    /// </summary>
    Task<Guid> Create(UsagePolicy entity, CancellationToken cancellationToken = default);
}
