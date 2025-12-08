using LabBooking.Application.Features.EquipmentCategories.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetByLabId;

public record GetCategoriesByLabIdQuery(Guid LabId) : IRequest<IEnumerable<EquipmentCategoryResponse>>;
