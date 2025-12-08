namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentRepository(LabBookingDbContext dbContext) : IEquipmentRepository
{
    public async Task<Guid> Create(Equipment entity)
    {
        dbContext.Equipments.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task DeleteAsync(Equipment entity)
    {
        dbContext.Equipments.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Equipment?> GetByIdAsync(Guid id)
    {
        var equipment = await dbContext.Equipments.FindAsync(id);
        return equipment;
    }

    public async Task UpdateAsync(Equipment equipment, CancellationToken token = default)
    {
        dbContext.Equipments.Update(equipment);

        await dbContext.SaveChangesAsync(token);
    }

    public async Task<(IEnumerable<Equipment>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ bản
        var baseQuery = dbContext
            .Equipments
            .Include(e => e.LabRoom) // Include Phòng Lab (nếu cần)

        // --- BẮT BUỘC THÊM DÒNG NÀY ---
        .Include(e => e.EquipmentCategory)
            .Where(e => searchPhraseLower == null ||
                        (e.EquipmentName.ToLower().Contains(searchPhraseLower)) ||
                        (e.Description != null && e.Description.ToLower().Contains(searchPhraseLower)));

        // 2. Đếm tổng số lượng
        var totalCount = await baseQuery.CountAsync();

        // 3. Xử lý Sắp xếp (SortBy)
        if (sortBy != null)
        {
            // Ánh xạ tên cột từ DTO/Query sang cột Entity
            var columnsSelector = new Dictionary<string, Expression<Func<Equipment, object>>>
            {
                { nameof(Equipment.EquipmentName), e => e.EquipmentName },
                { nameof(Equipment.Status), e => e.Status },
                { nameof(Equipment.IsAvailable), e => e.IsAvailable },
                { nameof(Equipment.LabRoomId), e => e.LabRoomId },
            };

            // Lấy cột được chọn từ dictionary
            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }
        else
        {
            // Sắp xếp mặc định (ví dụ: theo Tên) nếu không có SortBy
            baseQuery = baseQuery.OrderBy(e => e.EquipmentName);
        }

        // 4. Phân trang
        var equipments = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (equipments, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Equipments.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> IsEquipmentInLabAsync(Guid equipmentId, Guid labRoomId, CancellationToken token = default)
    {
        // Check xem có thiết bị nào ID như thế VÀ LabRoomId khớp không
        return await dbContext.Equipments
            .AnyAsync(e => e.Id == equipmentId && e.LabRoomId == labRoomId, token);
    }
}
