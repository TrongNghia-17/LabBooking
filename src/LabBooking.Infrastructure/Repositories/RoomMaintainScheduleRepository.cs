using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<Guid> Create(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext của bạn có DbSet tên là RoomMaintainSchedules
        dbContext.RoomMaintainSchedules.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id; // Trả về Id giống hệt LabRoomRepository
    }

    public async Task Update(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.RoomMaintainSchedules.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<RoomMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext của bạn có DbSet<RoomMaintainSchedule> tên là RoomMaintainSchedules
        var schedule = await dbContext.RoomMaintainSchedules.FindAsync(new object[] { id }, cancellationToken);
        return schedule;
    }

    public async Task<(IEnumerable<RoomMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        RoomMaintainStatus? status, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ sở
        var baseQuery = dbContext
            .RoomMaintainSchedules
            // Lọc theo SearchPhrase
            .Where(s => searchPhraseLower == null ||
                        (s.Description != null && s.Description.ToLower().Contains(searchPhraseLower)))

            // BỎ MỆNH ĐỀ .Where(s => labRoomId == null || s.LabRoomId == labRoomId)

            // 2. Lọc theo Status
            .Where(s => status == null || s.RoomMaintainStatus == status);

        // 3. Đếm tổng số lượng
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (giữ nguyên)
        if (sortBy != null)
        {
            // ... (logic sắp xếp giữ nguyên)
            var columnsSelector = new Dictionary<string, Expression<Func<RoomMaintainSchedule, object>>>
            {
                { nameof(RoomMaintainSchedule.StartTime), s => s.StartTime! },
                { nameof(RoomMaintainSchedule.EndTime), s => s.EndTime! },
                { nameof(RoomMaintainSchedule.RoomMaintainStatus), s => s.RoomMaintainStatus! }
            };

            if (columnsSelector.TryGetValue(sortBy, out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }
        else
        {
            baseQuery = baseQuery.OrderByDescending(s => s.StartTime);
        }

        // 5. Phân trang
        var schedules = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (schedules, totalCount);
    }
}
