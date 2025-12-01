using LabBooking.Domain.NonEntities;

namespace LabBooking.Domain.Repositories;

public interface IEquipmentMaintainScheduleRepository
{
    Task CreateAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
    Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
    Task<string> ProcessAutomatedMaintenanceAsync(CancellationToken token);
    Task<ScheduleConflictInfo?> GetConflictInfoAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token);
}
