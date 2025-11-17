namespace LabBooking.Infrastructure.Repositories;

/// <summary>
/// Implements the repository for UsagePolicy data.
/// </summary>
internal class UsagePolicyRepository(LabBookingDbContext dbContext) : IUsagePolicyRepository
{
    public async Task<Guid> Create(UsagePolicy entity, CancellationToken cancellationToken = default)
    {
        dbContext.UsagePolicies.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <summary>
    /// Lấy UsagePolicy bằng ID (tương tự LabRoomRepository)
    /// </summary>
    public async Task<UsagePolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var policy = await dbContext.UsagePolicies.FindAsync(new object[] { id }, cancellationToken);
        return policy;
    }

    /// <summary>
    /// Cập nhật một UsagePolicy (tương tự LabRoomRepository)
    /// </summary>
    public async Task Update(UsagePolicy entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Xóa một UsagePolicy (tương tự LabRoomRepository)
    /// </summary>
    public async Task DeleteAsync(UsagePolicy entity, CancellationToken cancellationToken = default)
    {
        dbContext.UsagePolicies.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Kiểm tra sự tồn tại của Policy bằng ID
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.UsagePolicies.AnyAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Kiểm tra Title có duy nhất không (khi tạo mới)
    /// (Tương tự IsLabNameUniqueAsync)
    /// </summary>
    public async Task<bool> IsTitleUniqueAsync(string title, CancellationToken cancellationToken = default)
    {
        var titleLower = title.ToLower();
        return !await dbContext.UsagePolicies
            .AnyAsync(r => r.Title != null && r.Title.ToLower() == titleLower, cancellationToken);
    }

    /// <summary>
    /// Kiểm tra Title có duy nhất không (khi cập nhật, loại trừ chính nó)
    /// (Tương tự IsLabNameUniqueAsync(Guid id, ...))
    /// </summary>
    public async Task<bool> IsTitleUniqueAsync(Guid id, string title, CancellationToken cancellationToken = default)
    {
        var isDuplicate = await dbContext.UsagePolicies
            .AnyAsync(policy => policy.Title == title && policy.Id != id, cancellationToken);

        return !isDuplicate;
    }

    public async Task<(IEnumerable<UsagePolicy>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        bool? isActive, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ sở
        var baseQuery = dbContext
            .UsagePolicies
            // Lọc theo SearchPhrase (tìm trong Title hoặc Description)
            .Where(p => searchPhraseLower == null ||
                        (p.Title != null && p.Title.ToLower().Contains(searchPhraseLower)) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchPhraseLower)))
            // 2. Lọc theo IsActive (ĐIỂM MỚI)
            .Where(p => isActive == null || p.IsActive == isActive);

        // 3. Đếm tổng số lượng (trước khi phân trang)
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (giống LabRoomRepository)
        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<UsagePolicy, object>>>
            {
                { nameof(UsagePolicy.Title), p => p.Title! },
                { nameof(UsagePolicy.CreatedDate), p => p.CreatedDate },
                { nameof(UsagePolicy.EffectiveFrom), p => p.EffectiveFrom! }
            };

            // Đảm bảo an toàn: chỉ sắp xếp nếu SortBy nằm trong danh sách
            if (columnsSelector.TryGetValue(sortBy, out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }
        else
        {
            // Sắp xếp mặc định
            baseQuery = baseQuery.OrderByDescending(p => p.CreatedDate);
        }

        // 5. Phân trang
        var policies = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (policies, totalCount);
    }
}
