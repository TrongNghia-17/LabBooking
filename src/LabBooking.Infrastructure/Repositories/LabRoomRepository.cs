namespace LabBooking.Infrastructure.Repositories;

internal class LabRoomRepository(LabBookingDbContext dbContext) : ILabRoomRepository
{
    public async Task<Guid> Create(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.LabRooms.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<LabRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var labRoom = await dbContext.LabRooms
            .Include(x => x.Equipments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return labRoom;
    }

    public async Task Update(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.LabRooms.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.LabRooms.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<LabRoom>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .LabRooms
            .Include(x => x.Equipments)
            .Include(r => r.MainManager)
            .Where(r => searchPhraseLower == null ||
                        (r.LabName != null && r.LabName.ToLower().Contains(searchPhraseLower)) ||
                        (r.Location != null && r.Location.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<LabRoom, object>>>
            {
                { nameof(LabRoom.LabName), r => r.LabName! },
                { nameof(LabRoom.Location), r => r.Location! },
                { nameof(LabRoom.CreatedDate), r => r.CreatedDate }
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var labRooms = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (labRooms, totalCount);
    }

    public async Task<IEnumerable<LabRoom>> GetUnmaintainedLabRoomsAsync(CancellationToken cancellationToken = default)
    {
        // Lấy danh sách LabRoomIds có lịch bảo trì với trạng thái là NotYet
        var labRoomIdsWithPendingMaintenance = dbContext.RoomMaintainSchedules
            .Where(s => s.RoomMaintainStatus == RoomMaintainStatus.NotYet)
            .Select(s => s.LabRoomId)
            .Distinct(); // Đảm bảo chỉ lấy ID duy nhất (vì một phòng có thể có nhiều lịch)

        // Lấy các LabRoom tương ứng
        var unmaintainedLabRooms = await dbContext.LabRooms
            .Where(r => !labRoomIdsWithPendingMaintenance.Contains(r.Id))
            .Include(lab => lab.Equipments)
            .ToListAsync(cancellationToken);

        return unmaintainedLabRooms;
    }

    public async Task<bool> IsLabNameUniqueAsync(string labName, CancellationToken cancellationToken = default)
    {
        var labNameLower = labName.ToLower();
        return !await dbContext.LabRooms
            .AnyAsync(r => r.LabName != null && r.LabName.ToLower() == labNameLower, cancellationToken);
    }

    public async Task<bool> IsLabNameUniqueAsync(Guid id, string labName, CancellationToken cancellationToken = default)
    {
        var isDuplicate = await dbContext.LabRooms
            .AnyAsync(room => room.LabName == labName && room.Id != id, cancellationToken);

        return !isDuplicate;
    }

    public async Task<IEnumerable<LabRoom>> GetLabsByManagerWithEquipmentsAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.LabRooms
            .Where(r => r.MainManagerId == managerId && r.IsActive)
            .Include(r => r.Equipments)
                .ThenInclude(e => e.EquipmentCategory)
            .OrderBy(r => r.LabName)
            .ToListAsync(cancellationToken);
    }
}
