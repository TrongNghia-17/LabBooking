namespace LabBooking.Application.Features.EquipmentCategories.Commands.Update;

public record UpdateEquipmentCategoryCommand(
    Guid Id, // ID của category cần sửa
    string Name,
    string? Description
) : IRequest; // Không cần trả về giá trị, hoặc trả về Unit
