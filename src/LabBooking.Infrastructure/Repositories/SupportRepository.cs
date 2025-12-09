namespace LabBooking.Infrastructure.Repositories;

internal class SupportRepository(LabBookingDbContext dbContext) : ISupportRepository
{
    public async Task<Guid> Create(Support entity, CancellationToken cancellationToken = default)
    {
        dbContext.Supports.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    // Trong file SupportRepository.cs

    public async Task<(IEnumerable<Support>, int)> GetAllMatchingAsync(
        string? searchPhrase,

        // --- THÊM THAM SỐ NÀY ---
        SupportStatus? status,
        // ------------------------

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
            .AsNoTracking() // Nên thêm AsNoTracking cho query dạng GET để tối ưu
            .AsQueryable();

        // 1. Lọc theo Search Phrase
        if (!string.IsNullOrWhiteSpace(searchPhraseLower))
        {
            baseQuery = baseQuery.Where(r =>
                r.Title.ToLower().Contains(searchPhraseLower) ||
                r.Content.ToLower().Contains(searchPhraseLower));
        }

        // 2. [MỚI] Lọc theo Status
        if (status.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.Status == status.Value);
        }

        // 3. Đếm tổng số (Total Count)
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (Sorting)
        if (!string.IsNullOrEmpty(sortBy))
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Support, object>>>
        {
            { nameof(Support.Title).ToLower(), r => r.Title },
            { nameof(Support.Content).ToLower(), r => r.Content },
            // Thêm sort theo Status luôn nếu muốn
            { "status", r => r.Status }
        };

            // Chuyển key về chữ thường để so sánh an toàn
            if (columnsSelector.TryGetValue(sortBy.ToLower(), out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
            else
            {
                // Mặc định sắp xếp ngày tạo mới nhất
                baseQuery = baseQuery.OrderByDescending(s => s.CreatedAt);
            }
        }
        else
        {
            baseQuery = baseQuery.OrderByDescending(s => s.CreatedAt);
        }

        // 5. Phân trang (Pagination)
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
