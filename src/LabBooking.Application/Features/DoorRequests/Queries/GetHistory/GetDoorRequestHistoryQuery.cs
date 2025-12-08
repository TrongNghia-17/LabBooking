using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetHistory;

public class GetDoorRequestHistoryQuery : IRequest<IEnumerable<DoorRequestHistoryDto>>
{
    // Các bộ lọc tùy chọn (Nullable)
    public Guid? LabRoomId { get; set; }
    public DoorRequestStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
