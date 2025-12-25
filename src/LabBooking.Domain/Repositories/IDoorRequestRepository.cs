namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<Guid> AddAsync(DoorOpeningRequest entity);
    Task<bool> HasPendingRequestAsync(
        string bookingCode,
        DateOnly requestDate,
        Guid slotId);
    Task<DoorOpeningRequest?> GetByIdAsync(Guid id);
    Task DeleteAsync(DoorOpeningRequest request);
    Task<(IEnumerable<DoorOpeningRequest> Items, int TotalCount)> GetPagedListAsync(
        Guid? managerId,      // Nếu có giá trị -> Lọc theo Manager
        Guid? requestedById,  // Nếu có giá trị -> Lọc theo Người tạo
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        DateOnly? filterDate,
        DoorRequestStatus? filterStatus, // Trạng thái cụ thể (Pending/Approved...)
        bool? isHistory,                 // [MỚI] True: Lấy (Approved + Rejected), False: Lấy Pending
        CancellationToken cancellationToken);
    Task UpdateAsync(DoorOpeningRequest request);
    Task<DoorOpeningRequest?> GetByIdWithUserAsync(Guid id);
}
