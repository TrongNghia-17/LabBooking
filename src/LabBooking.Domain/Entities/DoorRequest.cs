namespace LabBooking.Domain.Entities;

public enum DoorRequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Completed
}

public class DoorRequest
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid RequestedById { get; set; }
    [ForeignKey(nameof(RequestedById))]
    public User? RequestedBy { get; set; }

    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    public DoorRequestStatus Status { get; set; } = DoorRequestStatus.Pending;

    public Guid? HandledById { get; set; } // bảo vệ xử lý
    [ForeignKey(nameof(HandledById))]
    public User? HandledBy { get; set; }
}
