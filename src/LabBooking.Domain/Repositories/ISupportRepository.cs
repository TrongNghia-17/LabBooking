namespace LabBooking.Domain.Repositories;

public interface ISupportRepository
{
    Task<Support> Create(Support entity);

    Task<(IEnumerable<Support>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection);

    Task<Support?> GetByIdAsync(Guid id);
    Task Update(Support entity);

    Task DeleteAsync(Support entity);
}
