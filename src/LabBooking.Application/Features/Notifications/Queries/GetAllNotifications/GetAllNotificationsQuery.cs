using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Notifications.Dtos;

namespace LabBooking.Application.Features.Notifications.Queries.GetAllNotifications;

/// <summary>
/// Represents a query to retrieve a paginated, filtered, and sorted list of notifications.
/// </summary>
public record GetAllNotificationsQuery(
    string? SearchPhrase,
    bool? IsRead,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<NotificationsResponse>>;
