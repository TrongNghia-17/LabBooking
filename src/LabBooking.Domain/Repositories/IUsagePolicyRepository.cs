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

    /// <summary>
    /// Gets a usage policy by its unique identifier.
    /// </summary>
    Task<UsagePolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing usage policy in the database.
    /// </summary>
    Task Update(UsagePolicy entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a usage policy from the database.
    /// </summary>
    Task DeleteAsync(UsagePolicy entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a usage policy with the specified title already exists (for creation).
    /// </summary>
    Task<bool> IsTitleUniqueAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a usage policy with the specified title already exists, excluding the current policy (for update).
    /// </summary>
    Task<bool> IsTitleUniqueAsync(Guid id, string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a usage policy with the specified ID exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IEnumerable<UsagePolicy>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        bool? isActive, // Thêm tham số lọc
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default);
}
