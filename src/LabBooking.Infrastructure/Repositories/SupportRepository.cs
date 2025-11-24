namespace LabBooking.Infrastructure.Repositories;

internal class SupportRepository(LabBookingDbContext dbContext) : ISupportRepository
{
    public async Task<Guid> Create(Support entity, CancellationToken cancellationToken = default)
    {
        dbContext.Supports.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<(IEnumerable<Support>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .Supports
            .Where(r => searchPhraseLower == null ||
                        r.Title.ToLower().Contains(searchPhraseLower) ||
                        r.Content.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
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
            .ToListAsync(cancellationToken);

        return (supports, totalCount);
    }

    public async Task<Support?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var support = await dbContext.Supports.FindAsync(id, cancellationToken);
        return support;
    }

    public async Task Update(Support entity, CancellationToken cancellationToken = default)
    {
        dbContext.Supports.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Support entity, CancellationToken cancellationToken = default)
    {
        dbContext.Supports.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
