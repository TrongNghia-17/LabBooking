namespace LabBooking.Domain.Entities;

public class BookingSlot
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid BookingId { get; set; }
    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }


    public DateOnly Date { get; set; }  // Ngày

    public Guid SlotId { get; set; }
    [ForeignKey(nameof(SlotId))]
    public Slot? Slot { get; set; }

    public UnavailableReason Reason { get; set; } = UnavailableReason.Booked;
    public int Priority { get; set; } = 2;
    // 0 -> Maintenance
    // 1 -> UniversityEvent
    // 2 -> Standard

    public BookingSlotStatus Status { get; set; } = BookingSlotStatus.Active;
    public Guid? OverriddenByBookingId { get; set; }
}

public enum UnavailableReason
{
    Booked , //0
    Maintenance, //1
    PastTime,
    Locked
}

public enum BookingSlotStatus
{
    Active,
    Overridden, // [MỚI] Bị ghi đè, chờ xử lý
    Cancelled
}
