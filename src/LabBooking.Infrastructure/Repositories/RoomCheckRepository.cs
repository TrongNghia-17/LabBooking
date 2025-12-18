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
}
