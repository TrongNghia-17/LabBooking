namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentCategoryRepository(LabBookingDbContext dbContext) : IEquipmentCategoryRepository
{
    public async Task<EquipmentCategory?> GetByIdAsync(Guid id, CancellationToken token)
    {
        return await dbContext.EquipmentCategories.FindAsync(new object[] { id }, token);
    }

    public async Task UpdateAsync(EquipmentCategory category, CancellationToken token)
    {
        dbContext.EquipmentCategories.Update(category);
        await dbContext.SaveChangesAsync(token);
    }

    public async Task<bool> IsNameExistsExcludeIdAsync(string name, Guid excludeId, CancellationToken token)
    {
        return await dbContext.EquipmentCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != excludeId, token);
    }
    public async Task<Guid> CreateAsync(EquipmentCategory category, CancellationToken token)
    {
        await dbContext.EquipmentCategories.AddAsync(category, token);
        await dbContext.SaveChangesAsync(token);
        return category.Id;
    }

    public async Task<bool> IsNameExistsAsync(string name, CancellationToken token)
    {
        // Kiểm tra tên case-insensitive (không phân biệt hoa thường)
        return await dbContext.EquipmentCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), token);
    }
    public async Task<(IEnumerable<EquipmentCategory>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ bản & Include thiết bị để đếm số lượng
        var baseQuery = dbContext.EquipmentCategories
            .Include(c => c.Equipments) // Include để lấy list con
            .AsNoTracking()
            .AsQueryable();

        // 2. Lọc theo từ khóa (Search)
        if (!string.IsNullOrWhiteSpace(searchPhraseLower))
        {
            baseQuery = baseQuery.Where(c =>
                c.Name.ToLower().Contains(searchPhraseLower) ||
                (c.Description != null && c.Description.ToLower().Contains(searchPhraseLower)));
        }

        // 3. Đếm tổng số bản ghi (Total Count)
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (Sorting)
        if (!string.IsNullOrEmpty(sortBy))
        {
            // Map tên cột từ string sang biểu thức
            var columnsSelector = new Dictionary<string, Expression<Func<EquipmentCategory, object>>>
        {
            { "name", c => c.Name },
            { "description", c => c.Description ?? string.Empty },
            // Sắp xếp theo số lượng thiết bị
            { "equipmentCount", c => c.Equipments != null ? c.Equipments.Count : 0 }
        };

            // Nếu cột hợp lệ thì sort, nếu không mặc định sort theo Name
            if (columnsSelector.TryGetValue(sortBy.ToLower(), out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
            else
            {
                baseQuery = baseQuery.OrderBy(c => c.Name);
            }
        }
        else
        {
            baseQuery = baseQuery.OrderBy(c => c.Name);
        }

        // 5. Phân trang (Pagination)
        var categories = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (categories, totalCount);
    }
    public async Task<IEnumerable<EquipmentCategory>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            .Include(c => c.Equipments.Where(e => e.LabRoom.MainManagerId == managerId))
            .Where(c => c.Equipments.Any(e =>
                e.LabRoom != null &&
                e.LabRoom.MainManagerId == managerId))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Equipments
            .Include(e => e.LabRoom)
            .Where(e => e.EquipmentCategoryId == categoryId)
            .OrderBy(e => e.EquipmentName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentsByCategoryAndManagerAsync(
        Guid categoryId,
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Equipments
            .Include(e => e.LabRoom)
            .Where(e => e.EquipmentCategoryId == categoryId &&
                        e.LabRoom != null &&
                        e.LabRoom.MainManagerId == managerId)
            .OrderBy(e => e.EquipmentName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EquipmentCategory>> GetByLabIdAsync(Guid labId, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            // 1. Chỉ include những thiết bị thuộc về LabId này
            .Include(c => c.Equipments.Where(e => e.LabRoomId == labId))
            // 2. Chỉ lấy những Category có chứa ít nhất 1 thiết bị thuộc LabId này
            .Where(c => c.Equipments.Any(e => e.LabRoomId == labId))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }


}