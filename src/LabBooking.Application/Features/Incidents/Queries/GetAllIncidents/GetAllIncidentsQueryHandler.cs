using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetIncidentsQueryHandler(
    IIncidentRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetIncidentsQuery, IEnumerable<IncidentResponse>>
{
    public async Task<IEnumerable<IncidentResponse>> Handle(GetIncidentsQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin User
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");
        var roles = currentUserService.Roles.ToList();

        // 2. XÁC ĐỊNH SCOPE (PHẠM VI DỮ LIỆU)
        Guid? managerIdParam = null;

        if (roles.Contains("Admin") || roles.Contains("Guard"))
        {
            // Nếu là Admin hoặc Bảo vệ -> Xem toàn cục -> managerIdParam = null
            managerIdParam = null;
        }
        else if (roles.Contains("Manager"))
        {
            // Nếu là Manager -> Bị giới hạn phạm vi -> Truyền ID vào để Repo lọc
            managerIdParam = currentUserId;
        }
        else
        {
            // Sinh viên/Giảng viên không được gọi API này (hoặc chỉ xem của mình - logic khác)
            // Tạm thời trả về rỗng hoặc Throw Forbid tùy bạn
            return [];
        }

        // 3. GỌI REPO VỚI BỘ LỌC FULL OPTION
        var incidents = await repository.GetFilteredAsync(
            managerIdParam,
            request.LabRoomId,
            request.FromDate,
            request.ToDate,
            request.IsResolved,
            request.Importance,
            request.IsDescending,
            cancellationToken);

        // 4. MAP DATA
        var response = mapper.Map<IEnumerable<IncidentResponse>>(incidents);

        // (Tùy chọn) Ẩn thông tin người báo nếu là Guard (như bài trước)
        if (roles.Contains("Guard") && !roles.Contains("Manager") && !roles.Contains("Admin"))
        {
            foreach (var item in response)
            {
                // Guard chỉ quan tâm sự cố, không cần biết tên user báo cáo (trừ khi cần liên hệ)
                // Tùy nghiệp vụ đồ án của bạn, có thể comment dòng này lại nếu muốn hiện luôn.
                item.ReportedByName = "Người dùng";
                item.ReportedByPhone = null;
            }
        }

        return response;
    }
}