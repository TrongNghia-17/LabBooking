using LabBooking.Domain.Enums;
using LabBooking.Domain.NonEntities;

namespace LabBooking.Domain.Repositories;

public interface IEquipmentMaintainScheduleRepository
{
    Task CreateAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
    Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
    Task<string> ProcessAutomatedMaintenanceAsync(CancellationToken token);
    Task<ScheduleConflictInfo?> GetConflictInfoAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
    Task<IEnumerable<EquipmentMaintainSchedule>> GetByManagerIdAsync(
        Guid managerId,
        DateTime? from,
        DateTime? to,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        CancellationToken token = default);
    Task<EquipmentMaintainSchedule?> GetByIdWithDetailsAsync(Guid id, CancellationToken token);
    Task DeleteAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
}
