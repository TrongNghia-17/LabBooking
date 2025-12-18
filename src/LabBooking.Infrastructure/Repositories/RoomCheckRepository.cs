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

    public async Task AddRoomCheckTransactionAsync(
       RoomCheck roomCheck,
       Incident? incident,
       List<Equipment> updatedEquipments,
       CancellationToken token)
    {
        // Bắt đầu Transaction tại tầng Infrastructure
        using var transaction = await dbContext.Database.BeginTransactionAsync(token);
        try
        {
            // 1. Lưu RoomCheck (Header & Details)
            await dbContext.RoomChecks.AddAsync(roomCheck, token);

            // 2. Lưu Incident (Nếu có sự cố)
            if (incident != null)
            {
                await dbContext.Incidents.AddAsync(incident, token);
            }

            // 3. Cập nhật trạng thái thiết bị (Những máy bị hỏng)
            if (updatedEquipments != null && updatedEquipments.Any())
            {
                dbContext.Equipments.UpdateRange(updatedEquipments);
            }

            // 4. Commit tất cả
            await dbContext.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw; // Ném lỗi ra để Handler hoặc Middleware xử lý
        }
    }
}
