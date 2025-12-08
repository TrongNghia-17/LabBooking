using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetGuardPendingRequests;

public class GetGuardPendingRequestsHandler(
    IDoorRequestRepository repo
    ) : IRequestHandler<GetGuardPendingRequestsQuery, IEnumerable<GuardRequestDto>>
{
    public async Task<IEnumerable<GuardRequestDto>> Handle(GetGuardPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repo.GetPendingRequestsForGuardAsync(cancellationToken);

        // Map sang DTO
        return entities.Select(e => new GuardRequestDto
        {
            RequestId = e.Id,
            LabRoomName = e.LabRoom?.LabName ?? "Unknown",
            RequestTime = e.RequestTime,

            // Map thông tin sinh viên
            Name = e.RequestedBy?.UserName ?? "Không tên",
            Email = e.RequestedBy?.Email ?? "---",
            PhoneNumber = e.RequestedBy?.PhoneNumber ?? ""
        });
    }
}
