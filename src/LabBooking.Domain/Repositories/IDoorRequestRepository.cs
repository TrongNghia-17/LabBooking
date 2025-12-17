namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<Guid> AddAsync(DoorOpeningRequest entity);
    Task<bool> HasPendingRequestAsync(string bookingCode);
    Task<DoorOpeningRequest?> GetByIdAsync(Guid id);
    Task DeleteAsync(DoorOpeningRequest request);
}
