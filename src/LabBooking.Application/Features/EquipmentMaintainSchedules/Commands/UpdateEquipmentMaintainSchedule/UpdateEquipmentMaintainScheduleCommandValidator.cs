namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

public class UpdateEquipmentMaintainScheduleCommandValidator : AbstractValidator<UpdateEquipmentMaintainScheduleCommand>
{
    public UpdateEquipmentMaintainScheduleCommandValidator()
    {
        RuleFor(c => c.StartTime)
        .NotEmpty().WithMessage("Vui lòng nhập thời gian bắt đầu.");

        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");

        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }
}
