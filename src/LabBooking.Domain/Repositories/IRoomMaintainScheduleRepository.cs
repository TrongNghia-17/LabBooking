namespace LabBooking.Domain.Repositories;

public interface IRoomMaintainScheduleRepository
{
    Task<IEnumerable<RoomMaintainSchedule>> GetOverlappingSchedulesAsync(
        Guid labRoomId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    //Task<Guid> Create(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<Guid> CreateWithOverrideLogicAsync(RoomMaintainSchedule schedule, CancellationToken cancellationToken);
    Task Update(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<RoomMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<RoomMaintainSchedule>, int)> GetAllMatchingAsync(
    string? searchPhrase,
    RoomMaintainStatus? status,
    int pageSize,
    int pageNumber,
    string? sortBy,
    SortDirection sortDirection,
    Guid? managerId, // <--- Tham số này cho phép null (Admin truyền null)
    CancellationToken cancellationToken);
    /// <summary>
    /// Lấy danh sách lịch bảo trì đã hết thời gian (EndTime < DateTime.UtcNow) và đang ở trạng thái NotYet.
    /// </summary>
    Task<IEnumerable<RoomMaintainSchedule>> GetExpiredNotYetSchedulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật hàng loạt và lưu các thay đổi.
    /// </summary>
    Task UpdateRange(IEnumerable<RoomMaintainSchedule> schedules, CancellationToken cancellationToken = default);
}
