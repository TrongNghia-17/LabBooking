using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public record CreateEquipmentMaintainScheduleCommand(
    List<Guid> EquipmentIds,
    DateTime StartTime,
    DateTime EndTime,
    string Description
) : IRequest<EquipmentMaintainBatchResponse>;
