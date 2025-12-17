namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<Guid> AddAsync(DoorOpeningRequest entity);
    Task<bool> HasPendingRequestAsync(string bookingCode);
    Task<DoorOpeningRequest?> GetByIdAsync(Guid id);
    Task DeleteAsync(DoorOpeningRequest request);
    Task<(IEnumerable<DoorOpeningRequest> Items, int TotalCount)> GetRequestsByManagerAsync(
        Guid managerId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        DateOnly? filterDate,
        DoorRequestStatus? filterStatus,
        CancellationToken cancellationToken);
}
