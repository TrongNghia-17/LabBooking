namespace LabBooking.Domain.Repositories;

public interface IEquipmentMaintainScheduleRepository
{
    Task<Guid> Create(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task Update(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<EquipmentMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        EquimentpMaintainStatus? status, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default);
}
