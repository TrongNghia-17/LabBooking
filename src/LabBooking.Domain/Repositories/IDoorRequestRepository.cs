namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<bool> HasPendingRequestAsync(Guid userId, Guid labRoomId, CancellationToken token);
    Task CreateAsync(DoorOpeningRequest request, CancellationToken token);
}
