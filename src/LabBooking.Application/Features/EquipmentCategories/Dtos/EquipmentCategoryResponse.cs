namespace LabBooking.Application.Features.EquipmentCategories.Dtos;

public class EquipmentCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EquipmentCount { get; set; }

    // THÊM DÒNG NÀY: Để chứa danh sách thiết bị chi tiết
    public IEnumerable<EquipmentSimpleResponse> Equipments { get; set; } = new List<EquipmentSimpleResponse>();
}
