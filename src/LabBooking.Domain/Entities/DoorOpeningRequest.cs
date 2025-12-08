namespace LabBooking.Domain.Entities;

public class DoorOpeningRequest
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid RequestedById { get; set; }
    [ForeignKey(nameof(RequestedById))]
    public User? RequestedBy { get; set; }

    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedTime { get; set; }
    public DoorRequestStatus Status { get; set; } = DoorRequestStatus.Pending;
    public DoorRequestType Type { get; set; } = DoorRequestType.Open;

    public Guid? HandledById { get; set; } // bảo vệ xử lý
    [ForeignKey(nameof(HandledById))]
    public User? HandledBy { get; set; }

    public Guid? BookingId { get; set; }
    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }
}
