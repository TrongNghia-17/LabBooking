namespace LabBooking.Domain.Entities;

public class DoorOpeningRequest
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
    public string BookingCode { get; set; } = default!;
    public string Reason { get; set; } = default!;
    public string? ManagerNote { get; set; }

    public Guid RequestedById { get; set; }
    [ForeignKey(nameof(RequestedById))]
    public User? RequestedBy { get; set; }

    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedTime { get; set; }
    public DoorRequestStatus Status { get; set; } = DoorRequestStatus.Pending;

    public Guid? ManagerId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public User? Manager { get; set; }

    public void Process(User manager, DoorRequestStatus newStatus, string note)
    {
        Manager = manager;
        ManagerId = manager.Id;
        Status = newStatus;
        ManagerNote = note;
        AcceptedTime = DateTime.UtcNow;
    }
}
