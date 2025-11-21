namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;

/// <summary>
/// Command để xử lý logic xóa một EquipmentMaintainSchedule.
/// </summary>
public record DeleteEquipmentMaintainScheduleCommand(Guid Id) : IRequest<Unit>;
