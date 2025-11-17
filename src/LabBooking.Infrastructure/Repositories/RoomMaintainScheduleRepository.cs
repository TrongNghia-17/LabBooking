using Microsoft.EntityFrameworkCore;

namespace LabBooking.Infrastructure.Repositories;

internal class RoomMaintainScheduleRepository(LabBookingDbContext dbContext) : IRoomMaintainScheduleRepository
{
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
