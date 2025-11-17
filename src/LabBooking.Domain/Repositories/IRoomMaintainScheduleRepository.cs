namespace LabBooking.Domain.Repositories;

public interface IRoomMaintainScheduleRepository
{
    Task<Guid> Create(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task Update(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(RoomMaintainSchedule entity, CancellationToken cancellationToken = default);
    Task<RoomMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<RoomMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        RoomMaintainStatus? status, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default);
}
