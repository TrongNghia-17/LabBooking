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
        int pageNumber,
        int pageSize,
        CancellationToken token);
    Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetAllAsync(
        DateTime? fromDate,
        DateTime? toDate,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task<EquipmentMaintainSchedule?> GetByIdWithDetailsAsync(Guid id, CancellationToken token);
    Task DeleteAsync(EquipmentMaintainSchedule schedule, CancellationToken token);

    /// <summary>
    /// Hoàn thành một lịch bảo trì cụ thể theo cách thủ công.
    /// Bao gồm việc cập nhật trạng thái thiết bị và tự động đóng các sự cố liên quan.
    /// </summary>
    /// <param name="schedule">Đối tượng lịch trình cần hoàn thành.</param>
    /// <param name="token">Cancellation Token.</param>
    Task CompleteScheduleManuallyAsync(EquipmentMaintainSchedule schedule, CancellationToken token);
    Task<List<EquipmentMaintainSchedule>> GetRecentlyFinishedSchedulesAsync(int minutesLookback, CancellationToken token);
}
