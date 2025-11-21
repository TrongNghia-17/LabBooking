using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetEquipmentMaintainScheduleById;

/// <summary>
/// Record chứa ID để truy vấn một EquipmentMaintainSchedule.
/// </summary>
public record GetEquipmentMaintainScheduleByIdQuery(Guid Id) : IRequest<EquipmentMaintainScheduleResponse>;
