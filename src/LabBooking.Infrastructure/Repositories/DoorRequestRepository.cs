namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext) : IDoorRequestRepository
{
    public async Task<(IEnumerable<DoorRequest>, int)> GetAllMatchingAsync(
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        DoorRequestStatus? statusFilter)
    {
        var baseQuery = dbContext.DoorRequests.AsQueryable();

        if (statusFilter.HasValue)
            baseQuery = baseQuery.Where(r => r.Status == statusFilter.Value);

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<DoorRequest, object>>>
            {
                { nameof(DoorRequest.RequestTime), r => r.RequestTime },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var labs = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (labs, totalCount);
    }
}
