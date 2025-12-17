using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext, INotificationRepository notificationRepo) : IDoorRequestRepository
{
    public async Task<Guid> AddAsync(DoorOpeningRequest entity)
    {
        await dbContext.DoorOpeningRequests.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> HasPendingRequestAsync(string bookingCode)
    {
        return await dbContext.DoorOpeningRequests
            .AnyAsync(x => x.BookingCode == bookingCode && x.Status == DoorRequestStatus.Pending);
    }

    public async Task<DoorOpeningRequest?> GetByIdAsync(Guid id)
    {
        return await dbContext.DoorOpeningRequests
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task DeleteAsync(DoorOpeningRequest request)
    {
        dbContext.DoorOpeningRequests.Remove(request);
        await dbContext.SaveChangesAsync();
    }
}
