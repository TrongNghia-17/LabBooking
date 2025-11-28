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

    public async Task<string> ProcessAutoStatusUpdatesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        int startedCount = 0;
        int endedCount = 0;

        var schedulesToStart = await dbContext.EquipmentMaintainSchedules
            .Include(s => s.Equipment)
            .Where(s => s.StartTime <= now
                     && s.EndTime > now
                     && s.Equipment != null
                     && s.Equipment.Status != EquipmentStatus.Maintain)
            .ToListAsync(cancellationToken);

        foreach (var schedule in schedulesToStart)
        {
            if (schedule.Equipment != null)
            {
                schedule.Equipment.Status = EquipmentStatus.Maintain;
                schedule.Equipment.IsAvailable = false;
                startedCount++;
            }
        }

        var schedulesToEnd = await dbContext.EquipmentMaintainSchedules
            .Include(s => s.Equipment)
            .Where(s => s.EndTime <= now
                     && s.EquimentpMaintainStatus != EquimentpMaintainStatus.Done)
            .ToListAsync(cancellationToken);

        foreach (var schedule in schedulesToEnd)
        {
            schedule.EquimentpMaintainStatus = EquimentpMaintainStatus.Done;

            if (schedule.Equipment != null)
            {
                if (schedule.Equipment.Status == EquipmentStatus.Maintain)
                {
                    schedule.Equipment.Status = EquipmentStatus.Available;
                    schedule.Equipment.IsAvailable = true;
                }
            }
            endedCount++;
        }

        if (startedCount > 0 || endedCount > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return $"Job Report: Đã chuyển {startedCount} thiết bị sang 'Bảo trì' | Đã hoàn tất {endedCount} lịch bảo trì.";
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
        EquimentpMaintainStatus? status,
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
