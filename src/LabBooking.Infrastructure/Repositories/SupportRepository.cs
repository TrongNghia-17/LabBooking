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
            .Include(s => s.CreatedBy)
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
        return await dbContext.Supports
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Lấy tất cả yêu cầu hỗ trợ được tạo bởi một người dùng cụ thể.
    /// </summary>
    /// <param name="createdById">ID của người dùng đã tạo yêu cầu.</param>
    /// <returns>Danh sách các yêu cầu hỗ trợ.</returns>
    public async Task<IEnumerable<Support>> GetByCreatedByIdAsync(Guid createdById, CancellationToken cancellationToken = default)
    {
        // Lọc theo CreatedById và sắp xếp giảm dần theo CreatedAt để yêu cầu mới nhất nằm trên cùng
        return await dbContext.Supports
            .Include(s => s.CreatedBy)
            .Where(s => s.CreatedById == createdById)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
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
