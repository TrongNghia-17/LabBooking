using LabBooking.Application.Features.Notifications.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Notifications.Queries.GetAllNotifications;

public class GetAllNotificationsQueryHandler(
        ILogger<GetAllNotificationsQueryHandler> logger,
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : IRequestHandler<GetAllNotificationsQuery, PagedResult<NotificationsResponse>>
{
    public async Task<PagedResult<NotificationsResponse>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            logger.LogWarning("GetAllNotifications: User is not authenticated.");
            throw new UnauthorizedAccessException("Invalid user.");
        }

        logger.LogInformation(
            "Processing GetAllNotificationsQuery for User {UserId}: PageSize={PageSize}, PageNumber={PageNumber}, SortBy={SortBy}, IsRead={IsRead}, Search={Search}",
            userId.Value, request.PageSize, request.PageNumber, request.SortBy, request.IsRead, request.SearchPhrase);

        var (notifications, totalCount) = await notificationRepository.GetAllMatchingAsync(
            userId.Value,
            request.SearchPhrase,
            request.IsRead,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var notificationsResponse = mapper.Map<IEnumerable<NotificationsResponse>>(notifications);

        var result = new PagedResult<NotificationsResponse>(
            notificationsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
