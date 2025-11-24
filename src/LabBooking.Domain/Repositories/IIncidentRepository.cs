namespace LabBooking.Domain.Repositories;

public interface IIncidentRepository
{
    Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
       string? searchPhrase,
       int pageSize,
       int pageNumber,
       string? sortBy,
       SortDirection sortDirection);
    Task<Guid> Create(Incident entity, CancellationToken cancellationToken = default);
}
