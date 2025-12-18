using LabBooking.Application.Features.DoorRequests.Dtos;
using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext, INotificationRepository notificationRepo) : IDoorRequestRepository
{
    public async Task<Guid> AddAsync(DoorOpeningRequest entity)
    {
        await dbContext.DoorOpeningRequests.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> HasPendingRequestAsync(string bookingCode)
    {
        return await dbContext.DoorOpeningRequests
            .AnyAsync(x => x.BookingCode == bookingCode && x.Status == DoorRequestStatus.Pending);
    }

    public async Task<DoorOpeningRequest?> GetByIdAsync(Guid id)
    {
        return await dbContext.DoorOpeningRequests
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task DeleteAsync(DoorOpeningRequest request)
    {
        dbContext.DoorOpeningRequests.Remove(request);
        await dbContext.SaveChangesAsync();
    }

    public async Task<(IEnumerable<DoorOpeningRequest> Items, int TotalCount)> GetRequestsByManagerAsync(
        Guid managerId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        DateOnly? filterDate,
        DoorRequestStatus? filterStatus,
        CancellationToken cancellationToken)
    {
        // 1. Base Query: Chỉ lấy request thuộc về Manager này quản lý
        var baseQuery = dbContext.DoorOpeningRequests
            .Include(x => x.RequestedBy) // Include để lấy Email người gửi
            .Where(r => r.ManagerId == managerId)
            .AsNoTracking();

        // 2. Tìm kiếm (BookingCode hoặc Reason)
        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var lowerSearchPhrase = searchPhrase.ToLower();
            baseQuery = baseQuery.Where(r =>
                r.BookingCode.ToLower().Contains(lowerSearchPhrase) ||
                r.Reason.ToLower().Contains(lowerSearchPhrase));
        }

        // 3. Lọc theo ngày
        if (filterDate.HasValue)
        {
            // So sánh Date trong DateTime
            baseQuery = baseQuery.Where(r => DateOnly.FromDateTime(r.RequestTime) == filterDate.Value);
        }

        // 4. Lọc theo trạng thái (nếu có)
        if (filterStatus.HasValue)
        {
            baseQuery = baseQuery.Where(r => r.Status == filterStatus.Value);
        }

        // 5. Sắp xếp (Dynamic Sort đơn giản)
        // Mặc định sort theo RequestTime giảm dần (Mới nhất lên đầu)
        baseQuery = sortBy switch
        {
            nameof(DoorRequestDto.BookingCode) => sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(r => r.BookingCode)
                : baseQuery.OrderByDescending(r => r.BookingCode),

            nameof(DoorRequestDto.Status) => sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(r => r.Status)
                : baseQuery.OrderByDescending(r => r.Status),

            _ => sortDirection == SortDirection.Ascending // Mặc định là RequestTime
                ? baseQuery.OrderBy(r => r.RequestTime)
                : baseQuery.OrderByDescending(r => r.RequestTime)
        };

        // 6. Phân trang
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task UpdateAsync(DoorOpeningRequest request)
    {
        dbContext.DoorOpeningRequests.Update(request);
        await dbContext.SaveChangesAsync();
    }
}
