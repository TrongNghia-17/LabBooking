namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<bool> HasPendingRequestAsync(Guid userId, Guid labRoomId, CancellationToken token);
    Task DeleteAsync(DoorOpeningRequest request, CancellationToken token); // <--- Thêm dòng này
    Task CreateAsync(DoorOpeningRequest request, CancellationToken token);
    Task<IEnumerable<DoorOpeningRequest>> GetPendingRequestsForGuardAsync(CancellationToken token);
    Task<DoorOpeningRequest?> GetByIdAsync(Guid id, CancellationToken token);
    Task UpdateAsync(DoorOpeningRequest request, CancellationToken token);
    Task<IEnumerable<DoorOpeningRequest>> GetHistoryAsync(
    Guid currentUserId,
    bool canViewAll, // True: Xem hết, False: Chỉ xem của mình
    Guid? roomId,
    DoorRequestStatus? status,
    DateTime? from,
    DateTime? to,
    CancellationToken token);
}
