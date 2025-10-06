namespace LabBooking.Domain.Repositories;

public interface IDoorRequestRepository
{
    Task<(IEnumerable<DoorRequest>, int)> GetAllMatchingAsync(
      int pageSize,
      int pageNumber,
      string? sortBy,
      SortDirection sortDirection,
      DoorRequestStatus? status);
}
