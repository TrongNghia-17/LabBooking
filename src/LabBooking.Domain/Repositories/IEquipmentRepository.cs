namespace LabBooking.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Guid> Create(Equipment entity);
    Task<Equipment?> GetByIdAsync(Guid id);
    Task<List<Equipment>> GetByIdsAsync(List<Guid> ids, CancellationToken token);
    Task UpdateAsync(Equipment equipment, CancellationToken token = default);
    Task DeleteAsync(Equipment entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Equipment>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection);
    Task<bool> IsEquipmentInLabAsync(Guid equipmentId, Guid labRoomId, CancellationToken token = default);
    Task<int> GetMaintenanceCountAsync(Guid? managerId, CancellationToken token);
}
