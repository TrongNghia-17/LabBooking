namespace LabBooking.Application.Features.EquipmentCategories.Dtos;

public class EquipmentCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EquipmentCount { get; set; }
}
