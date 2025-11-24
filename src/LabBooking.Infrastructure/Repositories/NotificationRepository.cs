using LabBooking.Application.Features.Notifications.Dtos;

namespace LabBooking.Infrastructure.Repositories;

internal class NotificationRepository(LabBookingDbContext dbContext) : INotificationRepository
{
    public async Task<Guid> CreateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        entity.Id = Guid.NewGuid();
        dbContext.Notifications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<(IEnumerable<Notification>, int)> GetAllMatchingAsync(
            Guid userId,
            string? searchPhrase,
            bool? isRead,
            int pageSize,
            int pageNumber,
            string? sortBy,
            SortDirection sortDirection,
            CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ bản: CHỈ LẤY CỦA USER HIỆN TẠI
        var baseQuery = dbContext
            .Notifications
            .Where(n => n.UserId == userId);

        // 2. Lọc theo IsRead (nếu có)
        if (isRead.HasValue)
        {
            baseQuery = baseQuery.Where(n => n.IsRead == isRead.Value);
        }

        // 3. Lọc theo SearchPhrase
        if (searchPhraseLower != null)
        {
            baseQuery = baseQuery.Where(n =>
                n.Title.ToLower().Contains(searchPhraseLower) ||
                n.Message.ToLower().Contains(searchPhraseLower));
        }

        // 4. Lấy tổng số lượng (trước khi phân trang)
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 5. Sắp xếp (Sort)
        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Notification, object>>>
                {
                    { nameof(NotificationsResponse.CreatedAt), r => r.CreatedAt },
                    { nameof(NotificationsResponse.Title), r => r.Title },
                };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }
        else
        {
            // Mặc định sắp xếp mới nhất lên đầu
            baseQuery = baseQuery.OrderByDescending(n => n.CreatedAt);
        }

        // 6. Phân trang (Paginate)
        var notifications = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (notifications, totalCount);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications.FindAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        dbContext.Notifications.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
