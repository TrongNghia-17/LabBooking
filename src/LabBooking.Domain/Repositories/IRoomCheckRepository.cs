namespace LabBooking.Domain.Repositories;

public interface IRoomCheckRepository
{
    Task AddAsync(RoomCheck roomCheck, CancellationToken token);
    Task<RoomCheck?> GetByIdWithLabRoomAsync(Guid id, CancellationToken token);
    Task<RoomCheck?> GetByIdAsync(Guid id, CancellationToken token);
    Task SoftDeleteAsync(RoomCheck roomCheck, CancellationToken token);
    Task<bool> ExistsAsync(Guid labRoomId, Guid slotId, DateTime date, CancellationToken token);

}
