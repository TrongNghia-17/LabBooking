namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.Delete;

public class DeleteMaintainScheduleValidator : AbstractValidator<DeleteMaintainScheduleCommand>
{
    public DeleteMaintainScheduleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vui lòng cung cấp ID lịch trình.");
    }
}