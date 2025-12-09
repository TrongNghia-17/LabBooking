namespace LabBooking.Domain.Repositories;

public interface ISupportRepository
{
    Task<Guid> Create(Support entity, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Support>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        SupportStatus? status,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default);

    Task<Support?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Support>> GetByCreatedByIdAsync(Guid createdById, CancellationToken cancellationToken = default);
    Task Update(Support entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Support entity, CancellationToken cancellationToken = default);
}
