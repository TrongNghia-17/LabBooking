namespace LabBooking.Domain.Entities;

public class EquipmentCategory
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Equipment>? Equipments { get; set; }
}
