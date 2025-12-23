namespace LabBooking.Infrastructure.Repositories;

internal class RoomCheckRepository(LabBookingDbContext dbContext) : IRoomCheckRepository
{
    public async Task AddAsync(RoomCheck roomCheck, CancellationToken token)
    {
        // EF Core thông minh sẽ tự động Add cả RoomCheck, Details, 
        // và cả các Incident mới được link vào Details.
        await dbContext.RoomChecks.AddAsync(roomCheck, token);

        // Không gọi SaveChanges ở đây nếu muốn quản lý Transaction ở Handler
        // Nhưng nếu Repo chịu trách nhiệm lưu luôn thì gọi:
        await dbContext.SaveChangesAsync(token);
    }
    public async Task<RoomCheck?> GetByIdWithLabRoomAsync(Guid id, CancellationToken token)
    {
        return await dbContext.RoomChecks
            .AsNoTracking() // Tối ưu hiệu năng vì chỉ đọc dữ liệu
            .Include(rc => rc.LabRoom) // Include để lấy ManagerId và LabName
            .FirstOrDefaultAsync(rc => rc.Id == id, token);
    }

    public async Task<RoomCheck?> GetByIdAsync(Guid id, CancellationToken token)
    {
        return await dbContext.RoomChecks
            .FirstOrDefaultAsync(rc => rc.Id == id && !rc.IsDeleted, token);
    }

    public async Task SoftDeleteAsync(RoomCheck roomCheck, CancellationToken token)
    {
        roomCheck.IsDeleted = true;
        roomCheck.DeletedAt = DateTime.UtcNow;

        dbContext.RoomChecks.Update(roomCheck);
        await dbContext.SaveChangesAsync(token);
    }

    public async Task<bool> ExistsAsync(Guid labRoomId, Guid slotId, DateTime date, CancellationToken token)
    {
        // Lấy ngày hiện tại (bỏ phần giờ phút giây)
        var checkDate = date.Date;

        return await dbContext.RoomChecks
            .AnyAsync(rc => rc.LabRoomId == labRoomId
                         && rc.SlotId == slotId
                         && rc.CheckedAt.Date == checkDate // So sánh ngày
                         && !rc.IsDeleted, // Chỉ check những phiếu chưa xóa
                      token);
    }

    public async Task<bool> ExistsAsync(Guid labRoomId, Guid slotId, CheckType type, DateTime date, CancellationToken token)
    {
        var checkDate = date.Date;

        return await dbContext.RoomChecks
            .AnyAsync(rc => rc.LabRoomId == labRoomId
                         && rc.SlotId == slotId
                         && rc.Type == type // <--- [QUAN TRỌNG] Phải check cả Type
                         && rc.CheckedAt.Date == checkDate
                         && !rc.IsDeleted,
                      token);
    }

    public async Task<(IEnumerable<RoomCheck> Items, int TotalCount)> GetPagedListAsync(
        Guid userId,
        bool isManager,
        string? searchPhrase,
        CheckType? type,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken token)
    {
        // 1. Base Query & Include thông tin cần thiết
        var query = dbContext.RoomChecks
            .AsNoTracking()
            .Include(r => r.LabRoom)
            .Include(r => r.Guard) // Include để lấy tên bảo vệ hiển thị
            .Include(r => r.Slot)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        // 2. PHÂN QUYỀN (Logic quan trọng nhất)
        if (isManager)
        {
            // Manager: Chỉ xem các phiếu thuộc phòng Lab do mình quản lý
            query = query.Where(r => r.LabRoom.MainManagerId == userId);
        }
        else
        {
            // Bảo vệ (hoặc User khác): Chỉ xem phiếu do chính mình tạo
            query = query.Where(r => r.GuardId == userId);
        }

        // 3. Lọc theo Loại (CheckIn/CheckOut)
        if (type.HasValue)
        {
            query = query.Where(r => r.Type == type.Value);
        }

        // 4. Lọc theo Ngày
        if (fromDate.HasValue)
            query = query.Where(r => r.CheckedAt >= fromDate.Value.ToUniversalTime());

        if (toDate.HasValue)
            query = query.Where(r => r.CheckedAt <= toDate.Value.ToUniversalTime());

        // 5. Tìm kiếm (Search Phrase)
        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var lowerPhrase = searchPhrase.ToLower();
            query = query.Where(r =>
                // Tìm theo tên phòng
                r.LabRoom.LabName.ToLower().Contains(lowerPhrase) ||
                // Tìm theo ghi chú
                (r.Note != null && r.Note.ToLower().Contains(lowerPhrase))
            );
        }

        // 6. Sắp xếp (Mới nhất lên đầu)
        query = query.OrderByDescending(r => r.CheckedAt);

        // 7. Phân trang
        var totalCount = await query.CountAsync(token);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return (items, totalCount);
    }
}
