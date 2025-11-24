namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentMaintainScheduleRepository(LabBookingDbContext dbContext) : IEquipmentMaintainScheduleRepository
{
    public async Task<Guid> Create(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext có DbSet tên là EquipmentMaintainSchedules
        dbContext.EquipmentMaintainSchedules.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
    public async Task Update(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(EquipmentMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.EquipmentMaintainSchedules.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<EquipmentMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext có DbSet<EquipmentMaintainSchedule> tên là EquipmentMaintainSchedules
        var schedule = await dbContext.EquipmentMaintainSchedules.FindAsync(new object[] { id }, cancellationToken);
        return schedule;
    }

    public async Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        EquimentpMaintainStatus? status, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ sở
        var baseQuery = dbContext
            .EquipmentMaintainSchedules
            // Lọc theo SearchPhrase
            .Where(s => searchPhraseLower == null ||
                        (s.Description != null && s.Description.ToLower().Contains(searchPhraseLower)))

            // BỎ MỆNH ĐỀ .Where(s => equipmentId == null || s.EquipmentId == equipmentId)

            // 2. Lọc theo Status
            .Where(s => status == null || s.EquimentpMaintainStatus == status);

        // 3. Đếm tổng số lượng
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (giữ nguyên)
        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<EquipmentMaintainSchedule, object>>>
            {
                { "StartTime", s => s.StartTime! },
                { "EndTime", s => s.EndTime! },
                { "EquimentpMaintainStatus", s => s.EquimentpMaintainStatus! }
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
