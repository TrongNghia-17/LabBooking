namespace LabBooking.Domain.Repositories;

public interface IIncidentRepository
{
    Task<IEnumerable<Incident>> GetAllAsync();
    Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
       string? searchPhrase,
       int pageSize,
       int pageNumber,
       string? sortBy,
       SortDirection sortDirection);
    Task<Incident> Create(Incident entity);
}
