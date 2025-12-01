using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;

public record GetAllEquipmentCategoriesQuery : IRequest<IEnumerable<EquipmentCategoryResponse>>;
