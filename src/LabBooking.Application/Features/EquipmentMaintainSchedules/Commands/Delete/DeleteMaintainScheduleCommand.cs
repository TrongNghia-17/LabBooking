namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.Delete;

public record DeleteMaintainScheduleCommand(Guid Id) : IRequest<bool>;
