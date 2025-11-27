namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

/// <summary>
/// Command chứa dữ liệu để tạo một lịch bảo trì thiết bị mới.
/// </summary>
public record CreateEquipmentMaintainScheduleCommand(
    Guid EquipmentId,
    DateTime StartTime,
    DateTime EndTime,
    string Description
) : IRequest<Guid>;
