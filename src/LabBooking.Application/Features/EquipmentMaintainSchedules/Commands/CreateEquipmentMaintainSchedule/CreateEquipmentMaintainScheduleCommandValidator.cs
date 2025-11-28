namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandValidator : AbstractValidator<CreateEquipmentMaintainScheduleCommand>
{
    public CreateEquipmentMaintainScheduleCommandValidator()
    {
        RuleFor(c => c.EquipmentId)
            .NotEmpty().WithMessage("Vui lòng chọn thiết bị.");

        RuleFor(c => c.StartTime)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Thời gian bắt đầu bảo trì không được ở quá khứ.");

        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Vui lòng nhập mô tả bảo trì.")
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }
}
