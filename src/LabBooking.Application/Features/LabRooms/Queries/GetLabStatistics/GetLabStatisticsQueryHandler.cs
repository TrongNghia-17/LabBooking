using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetLabStatistics;

public class GetLabStatisticsQueryHandler(ILabRoomRepository repository)
    : IRequestHandler<GetLabStatisticsQuery, IEnumerable<LabStatisticResponse>>
{
    public async Task<IEnumerable<LabStatisticResponse>> Handle(GetLabStatisticsQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy dữ liệu thô (Domain Model) từ Repository
        var rawData = await repository.GetRawStatisticsAsync(request.Year, cancellationToken);

        // 2. Thực hiện GroupBy và tính toán ngay tại tầng Application (Use Case)
        // Đây là cách đúng: Logic tính toán nằm ở Application, Repo chỉ lấy dữ liệu.
        var result = rawData
            .GroupBy(x => new { x.LabId, x.LabName })
            .Select(g => new LabStatisticResponse
            {
                LabId = g.Key.LabId,
                LabName = g.Key.LabName,
                MonthlyData = Enumerable.Range(1, 12).Select(month => new MonthlyStatistic
                {
                    Month = month,

                    // Đếm số lần sử dụng (không phải bảo trì)
                    UsageCount = g.Where(x => x.Month == month && !x.IsMaintenance)
                                  .Select(x => x.BookingId)
                                  .Distinct() // Một booking chiếm nhiều slot chỉ tính là 1 lần
                                  .Count(),

                    // Đếm số lần bảo trì
                    MaintenanceCount = g.Where(x => x.Month == month && x.IsMaintenance)
                                        .Select(x => x.BookingId)
                                        .Distinct()
                                        .Count()
                }).ToList()
            })
            .OrderBy(r => r.LabName)
            .ToList();

        return result;
    }
}
