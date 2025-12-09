namespace LabBooking.Application.Features.EquipmentCategories.Commands.Create;

public record CreateEquipmentCategoryCommand(
    string Name,
    string? Description
) : IRequest<Guid>;
