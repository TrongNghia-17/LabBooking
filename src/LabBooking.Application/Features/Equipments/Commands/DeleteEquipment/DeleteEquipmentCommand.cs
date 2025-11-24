namespace LabBooking.Application.Features.Equipments.Commands.DeleteEquipment;

/// <summary>
/// Represents the command to delete an equipment by its ID.
/// </summary>
public record DeleteEquipmentCommand(Guid Id) : IRequest<Unit>;
