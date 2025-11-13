namespace LabBooking.Domain.Entities;

public class BookingSlot
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid BookingId { get; set; }
    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }


    public DateTime Date { get; set; }  // Ngày

    public Guid SlotId { get; set; }
    [ForeignKey(nameof(SlotId))]
    public Slot? Slot { get; set; }
}
