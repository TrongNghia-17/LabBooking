namespace LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

public record GetEquipmentsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<EquipmentSimpleResponse>>;
