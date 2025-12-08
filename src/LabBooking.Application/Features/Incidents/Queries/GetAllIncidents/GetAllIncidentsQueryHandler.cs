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
        var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var roles = currentUserService.Roles.ToList();

        // Biến cấu hình query
        Guid? filterManagerId = null;
        Guid? filterReporterId = null;
        bool hideSensitiveInfo = false; // Cờ ẩn tên/sđt

        // --- PHÂN QUYỀN ---

        if (roles.Contains("Admin"))
        {
            // Admin: Xem hết, không bị giới hạn gì cả
            filterManagerId = null;
            filterReporterId = null;
            hideSensitiveInfo = false;
        }
        else if (roles.Contains("Manager"))
        {
            // Manager: Chỉ xem phòng mình quản lý
            filterManagerId = currentUserId;
            filterReporterId = null;
            hideSensitiveInfo = false; // Manager cần thấy SĐT để liên hệ
        }
        else if (roles.Contains("Guard")) // Hoặc Security
        {
            // Bảo vệ: Xem hết (để đi tuần tra), nhưng có thể lọc theo LabRoomId từ Frontend gửi lên
            filterManagerId = null;
            filterReporterId = null;
            hideSensitiveInfo = true; // Yêu cầu của bạn: Bảo vệ không cần thấy tên/sđt người báo
        }
        else
        {
            // Sinh viên/Giảng viên: Chỉ xem cái mình tạo
            filterManagerId = null;
            filterReporterId = currentUserId;
            hideSensitiveInfo = false; // Xem của mình thì cứ hiện
        }

        // --- GỌI REPO ---
        var incidents = await repository.GetFilteredAsync(
            filterManagerId,   // Tham số quan trọng 1
            filterReporterId,  // Tham số quan trọng 2
            request.LabRoomId, // Filter từ FE
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
                item.ReportedByName = null;  // Ẩn
                item.ReportedByPhone = null; // Ẩn
            }
        }

        return response;
    }
}