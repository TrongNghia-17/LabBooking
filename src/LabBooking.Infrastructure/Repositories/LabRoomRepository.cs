namespace LabBooking.Infrastructure.Repositories;

internal class LabRoomRepository(LabBookingDbContext dbContext) : ILabRoomRepository
{
    public async Task<Guid> Create(LabRoom entity)
    {
        dbContext.LabRooms.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<LabRoom?> GetByIdAsync(Guid id)
    {
        var labRoom = await dbContext.LabRooms.FindAsync(id);
        return labRoom;
    }

    public async Task Update(LabRoom entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(LabRoom entity)
    {
        dbContext.LabRooms.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<(IEnumerable<LabRoom>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .LabRooms
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
            .ToListAsync();

        return (labRooms, totalCount);
    }
}
