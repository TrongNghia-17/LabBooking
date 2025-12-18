namespace LabBooking.Domain.Repositories;

public interface IRoomCheckRepository
{
    // Hàm này sẽ lưu RoomCheck + Details + Incidents + Equipment Updates
    Task AddAsync(RoomCheck roomCheck, CancellationToken token);
}
