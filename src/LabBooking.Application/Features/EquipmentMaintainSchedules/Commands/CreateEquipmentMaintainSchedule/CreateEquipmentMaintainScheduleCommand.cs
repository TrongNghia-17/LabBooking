using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public record CreateEquipmentMaintainScheduleCommand(
    Guid EquipmentId,
    DateTime StartTime,
    DateTime EndTime,
    string Description
) : IRequest<EquipmentMaintainScheduleResponse>;
