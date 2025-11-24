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
}
