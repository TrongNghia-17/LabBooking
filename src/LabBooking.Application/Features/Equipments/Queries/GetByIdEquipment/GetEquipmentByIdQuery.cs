using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.Equipments.Queries.GetByIdEquipment;

/// <summary>
/// Represents the query to retrieve a single equipment by its unique identifier.
/// </summary>
public record GetEquipmentByIdQuery(Guid Id) : IRequest<EquipmentResponse>;
