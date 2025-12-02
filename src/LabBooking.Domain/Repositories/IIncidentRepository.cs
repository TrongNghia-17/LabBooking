namespace LabBooking.Domain.Repositories;

public interface IIncidentRepository
{
    Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
       string? searchPhrase,
       int pageSize,
       int pageNumber,
       string? sortBy,
       SortDirection sortDirection);
    Task<IEnumerable<Incident>> GetFilteredAsync(
        Guid? managerId, // Null nếu là Guard/Admin, Có giá trị nếu là Manager
        Guid? labRoomId,
        DateTime? from,
        DateTime? to,
        bool? isResolved,
        LevelOfImportance? importance,
        bool isDescending,
        CancellationToken token);
    Task<Guid> CreateAsync(Incident incident, CancellationToken token);
    Task<bool> IsSpamAsync(Guid userId, Guid labRoomId, IncidentType type, CancellationToken token);
    Task<Incident?> GetByIdWithDetailsAsync(Guid id, CancellationToken token);
    Task DeleteAsync(Incident incident, CancellationToken token);
    Task<IEnumerable<Incident>> GetByReporterIdAsync(Guid reporterId, CancellationToken token);
    Task<IEnumerable<Incident>> GetByManagerIdAsync(Guid managerId, CancellationToken token);
}
