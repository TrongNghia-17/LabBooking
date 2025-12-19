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
}
