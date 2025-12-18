namespace LabBooking.Domain.Repositories;

public interface IRoomCheckRepository
{
    // Hàm này sẽ lưu RoomCheck + Details + Incidents + Equipment Updates
    Task AddAsync(RoomCheck roomCheck, CancellationToken token);

    /// <summary>
    /// Lưu RoomCheck, đồng thời lưu Incident (nếu có) và cập nhật trạng thái các thiết bị hỏng.
    /// Tất cả chạy trong 1 Transaction.
    /// </summary>
    Task AddRoomCheckTransactionAsync(
        RoomCheck roomCheck,
        Incident? incident,
        List<Equipment> updatedEquipments,
        CancellationToken token);
}
