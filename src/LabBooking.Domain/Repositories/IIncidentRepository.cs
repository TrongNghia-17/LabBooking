namespace LabBooking.Domain.Repositories;

public interface IIncidentRepository
{
    Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
       string? searchPhrase,
       int pageSize,
       int pageNumber,
       string? sortBy,
       SortDirection sortDirection);
    Task<Guid> CreateAsync(Incident incident, CancellationToken token);
    Task<bool> IsSpamAsync(Guid userId, Guid labRoomId, IncidentType type, CancellationToken token);
}
