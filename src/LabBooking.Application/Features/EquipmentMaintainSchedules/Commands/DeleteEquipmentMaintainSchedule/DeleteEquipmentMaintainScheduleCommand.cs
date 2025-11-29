namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;

public record DeleteEquipmentMaintainScheduleCommand(Guid Id) : IRequest<Unit>;
