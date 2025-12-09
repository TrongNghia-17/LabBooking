using LabBooking.Domain.NonEntities;

namespace LabBooking.Domain.Repositories;

public interface IEquipmentMaintainScheduleRepository
{
    Task CreateAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
    Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
    Task<string> ProcessAutomatedMaintenanceAsync(CancellationToken token);
    Task<ScheduleConflictInfo?> GetConflictInfoAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
    Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetByManagerIdAsync(
        Guid userId,
        DateTime? fromDate,
        DateTime? toDate,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        int pageNumber,  // <--- Thêm tham số
        int pageSize,    // <--- Thêm tham số
        CancellationToken token);
    Task<EquipmentMaintainSchedule?> GetByIdWithDetailsAsync(Guid id, CancellationToken token);
    Task DeleteAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
}
