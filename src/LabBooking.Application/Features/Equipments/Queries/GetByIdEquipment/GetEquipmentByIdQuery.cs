using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetByIdEquipment;

public record GetEquipmentByIdQuery(Guid Id) : IRequest<EquipmentResponse>;
