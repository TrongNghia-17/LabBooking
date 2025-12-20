using LabBooking.Application.Common.Interfaces;
using System.Globalization;

namespace LabBooking.Application.Features.Incidents.Queries.GetStatistics;

public class GetIncidentStatisticsQueryHandler(
    IIncidentRepository incidentRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetIncidentStatisticsQuery, IEnumerable<IncidentStatisticResponse>>
{
    public async Task<IEnumerable<IncidentStatisticResponse>> Handle(GetIncidentStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Kiểm tra quyền: Admin được xem tất cả, Manager chỉ xem của mình
        Guid? managerId = null;
        if (!currentUserService.Roles.Contains(Roles.Admin))
        {
            managerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        }

        // Gọi phương thức repository đã tạo
        var monthlyCounts = await incidentRepository.GetMonthlyIncidentStatsAsync(request.Year, managerId, cancellationToken);

        // Chuyển đổi kết quả sang Response DTO (thêm tên tháng)
        var response = monthlyCounts.Select(item => new IncidentStatisticResponse
        {
            MonthNumber = item.Month,
            // Lấy tên tháng đầy đủ dựa trên văn hóa hiện tại của server
            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month),
            IncidentCount = item.Count
        }).ToList();

        return response;
    }
}