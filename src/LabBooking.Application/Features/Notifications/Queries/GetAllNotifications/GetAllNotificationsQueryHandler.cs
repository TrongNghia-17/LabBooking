using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Notifications.Dtos;
using System.Text.Json;

namespace LabBooking.Application.Features.Notifications.Queries.GetAllNotifications;

public class GetAllNotificationsQueryHandler(
        ILogger<GetAllNotificationsQueryHandler> logger,
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IBookingConsentRequestRepository consentRepository,
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

        // 1. Lấy dữ liệu
        var (notifications, totalCount) = await notificationRepository.GetAllMatchingAsync(
            userId.Value,
            request.SearchPhrase,
            request.IsRead,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var notificationsResponse = mapper.Map<List<NotificationsResponse>>(notifications);

        // =================================================================================
        // 2. [LOGIC MỚI] CẬP NHẬT TRẠNG THÁI (BULK UPDATE) - ĐÃ FIX BUG CASE SENSITIVE
        // =================================================================================

        // Bước A: Lọc notification.
        // FIX 1: Check cả 2 trường hợp hoa/thường hoặc dùng IndexOf case-insensitive
        var consentNotis = notificationsResponse
            .Where(n => !string.IsNullOrEmpty(n.DataPayload) &&
                       (n.DataPayload.Contains("ConsentRequestId") || n.DataPayload.Contains("consentRequestId")))
            .ToList();

        if (consentNotis.Count != 0)
        {
            var consentIdsToFetch = new List<Guid>();
            // Dùng Dictionary để map ngược lại cho nhanh
            var notiPayloadMap = new Dictionary<NotificationsResponse, Dictionary<string, object>>();

            foreach (var noti in consentNotis)
            {
                try
                {
                    var payloadDict = JsonSerializer.Deserialize<Dictionary<string, object>>(noti.DataPayload!);

                    object? idObj = null;
                    if (payloadDict != null)
                    {
                        // Check Key an toàn (Ưu tiên chữ thường trước vì JSON thường là camelCase)
                        if (payloadDict.TryGetValue("consentRequestId", out var val1)) idObj = val1;
                        else if (payloadDict.TryGetValue("ConsentRequestId", out var val2)) idObj = val2;
                    }

                    if (idObj != null && Guid.TryParse(idObj.ToString(), out Guid consentId))
                    {
                        consentIdsToFetch.Add(consentId);
                        notiPayloadMap[noti] = payloadDict!;
                    }
                }
                catch { /* Ignore json error */ }
            }

            // Bước B: Gọi Repo (Bulk)
            if (consentIdsToFetch.Count != 0)
            {
                var statusMap = await consentRepository.GetStatusesAsync(consentIdsToFetch.Distinct().ToList(), cancellationToken);

                // Bước C: Map ngược lại
                foreach (var noti in consentNotis)
                {
                    if (notiPayloadMap.TryGetValue(noti, out var payloadDict))
                    {
                        // FIX 2: Lấy lại ID một cách an toàn (không fix cứng key)
                        object? idObj = null;
                        if (payloadDict.TryGetValue("consentRequestId", out var val1)) idObj = val1;
                        else if (payloadDict.TryGetValue("ConsentRequestId", out var val2)) idObj = val2;

                        if (idObj != null && Guid.TryParse(idObj.ToString(), out Guid consentId))
                        {
                            if (statusMap.TryGetValue(consentId, out string? status))
                            {
                                // Xóa key cũ nếu có để tránh trùng
                                if (payloadDict.ContainsKey("consentStatus")) payloadDict.Remove("consentStatus");

                                // Ghi đè Status mới
                                payloadDict["consentStatus"] = status;

                                // Serialize lại
                                noti.DataPayload = JsonSerializer.Serialize(payloadDict);
                            }
                        }
                    }
                }
            }
        }
        // =================================================================================

        var result = new PagedResult<NotificationsResponse>(
            notificationsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
