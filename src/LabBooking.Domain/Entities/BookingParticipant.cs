namespace LabBooking.Domain.Entities;

public class BookingParticipant
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid BookingId { get; set; }
    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public bool IsApproved { get; set; } = false;
    public string Email { get; set; } = default!;
}
