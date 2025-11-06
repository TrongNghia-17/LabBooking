namespace LabBooking.Domain.Entities;

public class LabRoom
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    [Required]
    public string? LabName { get; set; } = string.Empty;

    public string? Location { get; set; }
    public int? MaximumLimit { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? MainManagerId { get; set; }
    [ForeignKey(nameof(MainManagerId))]
    public User? MainManager { get; set; }

    public Guid? CreatedById { get; set; }
    [ForeignKey(nameof(CreatedById))]
    public User? CreatedBy { get; set; }

    public ICollection<Equipment>? Equipments { get; set; }
    public ICollection<Incident>? Incidents { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
