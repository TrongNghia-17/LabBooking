using LabBooking.Domain.NonEntities;

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
         Guid? managerId,   // Nếu != null -> Chỉ lấy phòng do ông này quản lý
         Guid? reporterId,  // Nếu != null -> Chỉ lấy incident do ông này tạo
         Guid? labRoomId,
         string? searchPhrase,// Lọc theo phòng cụ thể
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

    /// <summary>
    /// Xóa mềm Incident và Phục hồi trạng thái thiết bị về Available (trong 1 transaction)
    /// </summary>
    Task SoftDeleteWithRestoreDevicesAsync(Incident incident, CancellationToken token);

    // Hàm hỗ trợ cho RoomCheck: Kiểm tra xem RoomCheck này có Incident nào đang dính không
    Task<bool> HasActiveIncidentForRoomCheckAsync(Guid roomCheckId, CancellationToken token);

    /// <summary>
    /// Lấy thống kê số lượng sự cố theo từng tháng trong một năm cụ thể.
    /// </summary>
    /// <param name="year">Năm cần thống kê.</param>
    /// <param name="managerId">Nếu có, chỉ thống kê các sự cố trong phòng do manager này quản lý.</param>
    /// <param name="token">Cancellation Token.</param>
    /// <returns>Danh sách thống kê theo tháng.</returns>
    Task<IEnumerable<MonthlyIncidentCount>> GetMonthlyIncidentStatsAsync(int year, Guid? managerId, CancellationToken token);

}
