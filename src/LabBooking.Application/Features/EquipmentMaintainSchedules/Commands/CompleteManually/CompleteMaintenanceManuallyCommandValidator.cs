namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CompleteManually;

public class CompleteMaintenanceManuallyCommandValidator : AbstractValidator<CompleteMaintenanceManuallyCommand>
{
    public CompleteMaintenanceManuallyCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("ID của lịch trình không được để trống.");
    }
}
