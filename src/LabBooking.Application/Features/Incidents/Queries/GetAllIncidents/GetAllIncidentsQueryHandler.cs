using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetIncidentsQueryHandler(
    IIncidentRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetIncidentsQuery, IEnumerable<IncidentResponse>>
{
    public async Task<IEnumerable<IncidentResponse>> Handle(GetIncidentsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var roles = currentUserService.Roles.ToList();

        Guid? filterManagerId = null;
        Guid? filterReporterId = null;
        bool hideSensitiveInfo = false;

        // --- LOGIC PHÂN QUYỀN ĐÃ SỬA LẠI ---
        // Ưu tiên quyền cao nhất trước. Nếu là Admin thì các quyền khác không cần xét nữa.
        if (roles.Contains("Admin"))
        {
            // Admin: Có toàn quyền, không cần filter gì cả.
        }
        else if (roles.Contains("Manager"))
        {
            // Manager: Bị giới hạn bởi các phòng mình quản lý.
            filterManagerId = currentUserId;
            // Quan trọng: Một manager cũng có thể tự báo cáo sự cố, nên không cần set filterReporterId.
            // Họ sẽ thấy tất cả sự cố trong phòng của họ, bao gồm cả sự cố do chính họ tạo.
        }
        else if (roles.Contains("SecurityGuard"))
        {
            // SecurityGuard (và các role khác như Student/Lecturer):
            // Chỉ thấy các sự cố do chính mình tạo.
            filterReporterId = currentUserId;
            hideSensitiveInfo = true;
        }
        else
        {
            // Mặc định cho các vai trò khác (Student, Lecturer): chỉ xem của mình
            filterReporterId = currentUserId;
        }

        // --- GỌI REPO ---
        var incidents = await repository.GetFilteredAsync(
            filterManagerId,
            filterReporterId,
            request.LabRoomId,
            request.SearchPhrase,
            request.FromDate,
            request.ToDate,
            request.IsResolved,
            request.Importance,
            request.IsDescending,
            cancellationToken);

        // --- MAP & ẨN THÔNG TIN ---
        var response = mapper.Map<IEnumerable<IncidentResponse>>(incidents);

        if (hideSensitiveInfo)
        {
            foreach (var item in response)
            {
                item.ReportedByName = null;
                item.ReportedByPhone = null;
            }
        }

        return response;
    }
}