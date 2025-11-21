using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class RoomMaintainScheduleRepository(LabBookingDbContext dbContext) : IRoomMaintainScheduleRepository
    {
        public async Task<IEnumerable<RoomMaintainSchedule>> GetOverlappingSchedulesAsync(
            Guid labRoomId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken)
        {
            // --- SỬA LỖI Ở ĐÂY ---

            // 1. Tạo ngày bắt đầu (T2 00:00:00) ở múi giờ Local của server
            var localQueryStartDate = startDate.ToDateTime(TimeOnly.MinValue);

            // 2. Tạo ngày kết thúc (T2 tuần sau 00:00:00) ở múi giờ Local
            var localQueryEndDate = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

            // 3. Chuyển đổi cả hai sang UTC để query
            // (Giả sử server chạy ở UTC+7, 00:00 Local -> 17:00 (ngày hôm trước) UTC)
            var queryStartDateUtc = localQueryStartDate.ToUniversalTime();
            var queryEndDateUtc = localQueryEndDate.ToUniversalTime();

            // --- KẾT THÚC SỬA ---

            // Logic tìm chồng chéo (overlap) giờ đã đúng múi giờ
            // (Schedule.StartTime < queryEndDateUtc) AND (Schedule.EndTime > queryStartDateUtc)

            var schedules = await dbContext.RoomMaintainSchedules
                .Where(m =>
                    m.LabRoomId == labRoomId &&
                    m.StartTime < queryEndDateUtc &&  // So sánh (timestamptz < Utc)
                    m.EndTime > queryStartDateUtc &&  // So sánh (timestamptz > Utc)
                    m.RoomMaintainStatus == RoomMaintainStatus.NotYet
                )
                .ToListAsync(cancellationToken);

            return schedules;
        }
    }
}
