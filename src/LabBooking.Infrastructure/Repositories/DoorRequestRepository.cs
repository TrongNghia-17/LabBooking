using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext) : IDoorRequestRepository
{
    public async Task<bool> HasPendingRequestAsync(Guid userId, Guid labRoomId, CancellationToken token)
    {
        // Kiểm tra xem User này có đang treo yêu cầu nào ở phòng này không
        return await dbContext.DoorOpeningRequests
            .AnyAsync(x => x.RequestedById == userId
                        && x.LabRoomId == labRoomId
                        && x.Status == DoorRequestStatus.Pending, token);
    }

    public async Task CreateAsync(DoorOpeningRequest request, CancellationToken token)
    {
        await dbContext.DoorOpeningRequests.AddAsync(request, token);
        await dbContext.SaveChangesAsync(token);
    }
}
