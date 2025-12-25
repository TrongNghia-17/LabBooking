using LabBooking.Domain.NonEntities;

namespace LabBooking.Application.Features.LabRooms.Queries.GetDailyLabSchedule;

public class GetDailyLabScheduleQueryHandler(
    ILabRoomRepository labRoomRepository // Inject Repository thay vì DbContext
    ) : IRequestHandler<GetDailyLabScheduleQuery, List<LabDailySchedule>>
{
    public async Task<List<LabDailySchedule>> Handle(GetDailyLabScheduleQuery request, CancellationToken cancellationToken)
    {
        // 1. Xác định ngày (Mặc định hôm nay nếu null)
        var targetDate = request.Date ?? DateOnly.FromDateTime(DateTime.Now);

        return await labRoomRepository.GetDailyScheduleAsync(targetDate, cancellationToken);
    }
}
