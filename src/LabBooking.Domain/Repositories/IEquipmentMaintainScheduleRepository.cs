namespace LabBooking.Domain.Repositories;

public interface IEquipmentMaintainScheduleRepository
{
    Task<Guid> Create(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task Update(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<string> ProcessAutoStatusUpdatesAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<EquipmentMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        EquimentpMaintainStatus? status,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default);
    Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token = default);
    Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, Guid? excludeScheduleId = null, CancellationToken token = default);
}
