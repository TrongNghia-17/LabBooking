namespace LabBooking.Domain.Repositories;

public interface INotificationRepository
{
    Task<(IEnumerable<Notification>, int)> GetAllMatchingAsync(
            Guid userId,
            string? searchPhrase,
            bool? isRead,
            int pageSize,
            int pageNumber,
            string? sortBy,
            SortDirection sortDirection,
            CancellationToken cancellationToken = default);

    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(Notification entity, CancellationToken cancellationToken = default);

    (Notification Entity, PushNotificationData PushData) PrepareNotification(
        Guid? userId,
        string title,
        string message,
        string type,
        object? extraData = null
    );

    // [HÀM 2] Chạy Background Task để gửi Push (Fire-and-forget)
    void RunPushNotificationTask(List<PushNotificationData> queue);
}

public record PushNotificationData(Guid? UserId, string Title, string Body, object Payload);
