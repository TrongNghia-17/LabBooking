namespace LabBooking.Infrastructure.Repositories;

internal class SupportRepository(LabBookingDbContext dbContext) : ISupportRepository
{
    public async Task<Support> Create(Support entity)
    {
        dbContext.Supports.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<(IEnumerable<Support>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Thay đổi logic filter (Where) cho Support (tìm theo Title và Content)
        var baseQuery = dbContext
            .Supports
            .Where(r => searchPhraseLower == null ||
                        r.Title.ToLower().Contains(searchPhraseLower) ||
                        r.Content.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            // 2. Thay đổi các cột dùng để sort
            // Dựa trên file Support.cs, chúng ta dùng Title và Content
            var columnsSelector = new Dictionary<string, Expression<Func<Support, object>>>
            {
                { nameof(Support.Title), r => r.Title },
                { nameof(Support.Content), r => r.Content },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var supports = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (supports, totalCount);
    }

    public async Task<Support?> GetByIdAsync(Guid id)
    {
        var support = await dbContext.Supports.FindAsync(id);
        return support;
    }

    public async Task Update(Support entity)
    {
        dbContext.Supports.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Support entity)
    {
        dbContext.Supports.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
