using LabBooking.Application.Features.Notifications.Dtos;
using LabBooking.Application.Interfaces.Notifications;

namespace LabBooking.Infrastructure.Repositories;

internal class NotificationRepository(LabBookingDbContext dbContext, IServiceScopeFactory scopeFactory) : INotificationRepository
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

    public (Notification Entity, PushNotificationData PushData) PrepareNotification(
            Guid? userId, string title, string message, string type, object? extraData = null)
    {
        var payloadObj = new
        {
            type,
            // Có thể thêm timestamp hoặc gì đó chung chung
            extra = extraData
        };

        var noti = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            DataPayload = JsonSerializer.Serialize(extraData ?? payloadObj) // Nếu extraData là object chuẩn thì dùng luôn
        };

        // Add vào Context hiện tại (để chờ SaveChanges chung với Booking)
        dbContext.Notifications.Add(noti);

        var pushData = new PushNotificationData(userId, title, message, extraData ?? payloadObj);

        return (noti, pushData);
    }

    // Hàm 2: Bắn Push ngầm (Chạy trên Background Thread)
    public void RunPushNotificationTask(List<PushNotificationData> queue)
    {
        if (queue == null || !queue.Any()) return;

        // Fire-and-forget: Không await để không chặn luồng chính
        _ = Task.Run(async () =>
        {
            // ⚠️ QUAN TRỌNG: Phải tạo Scope mới vì DbContext của Request chính sắp bị Dispose
            using var scope = scopeFactory.CreateScope();

            // Resolve lại các service cần thiết trong Scope mới
            var userDeviceRepo = scope.ServiceProvider.GetRequiredService<IUserDeviceRepository>();
            var notiService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            foreach (var item in queue)
            {
                try
                {
                    var tokens = await userDeviceRepo.GetTokensByUserIdAsync(item.UserId, default);
                    if (tokens != null && tokens.Any())
                    {
                        await notiService.SendPushNotificationAsync(tokens, item.Title, item.Body, item.Payload);
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi background (Console hoặc ILogger nếu inject)
                    Console.WriteLine($"[Background Push Error] User {item.UserId}: {ex.Message}");
                }
            }
        });
    }
}
