namespace LabBooking.Domain.Entities;

public class LabRoom
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    [Required]
    public string LabName { get; set; } = string.Empty;

    public string? Location { get; set; }

    public Guid? MainManagerId { get; set; }
    [ForeignKey(nameof(MainManagerId))]
    public User? MainManager { get; set; }

    public ICollection<Equipment>? Equipments { get; set; }
    public ICollection<Incident>? Incidents { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
