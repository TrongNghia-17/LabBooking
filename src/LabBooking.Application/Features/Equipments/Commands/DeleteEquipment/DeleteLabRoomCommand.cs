namespace LabBooking.Application.Features.Equipments.Commands.DeleteEquipment;

/// <summary>
/// Command để xử lý logic xóa một Equipment.
/// </summary>
public record DeleteEquipmentCommand(Guid Id) : IRequest<Unit>;
