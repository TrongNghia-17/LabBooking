namespace LabBooking.Domain.Repositories;

public interface IRoomCheckRepository
{
    // Hàm này sẽ lưu RoomCheck + Details + Incidents + Equipment Updates
    Task AddAsync(RoomCheck roomCheck, CancellationToken token);
    Task<RoomCheck?> GetByIdWithLabRoomAsync(Guid id, CancellationToken token);
    Task<RoomCheck?> GetByIdAsync(Guid id, CancellationToken token);
    Task SoftDeleteAsync(RoomCheck roomCheck, CancellationToken token);
}
