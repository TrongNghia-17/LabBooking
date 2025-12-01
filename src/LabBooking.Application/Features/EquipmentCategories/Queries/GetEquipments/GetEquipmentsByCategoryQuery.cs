using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

public record GetEquipmentsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<EquipmentSimpleResponse>>;
