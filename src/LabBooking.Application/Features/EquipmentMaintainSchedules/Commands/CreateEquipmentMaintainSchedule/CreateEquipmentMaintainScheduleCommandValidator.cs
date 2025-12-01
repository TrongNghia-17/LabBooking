namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandValidator : AbstractValidator<CreateEquipmentMaintainScheduleCommand>
{
    public CreateEquipmentMaintainScheduleCommandValidator(IEquipmentMaintainScheduleRepository repo)
    {
        RuleFor(c => c.EquipmentIds).NotEmpty().WithMessage("Chọn ít nhất 1 thiết bị.");
        RuleFor(c => c.StartTime)
        .Must(startTime =>
        {
            // Logic kiểm tra: Input (UTC) phải lớn hơn Hiện tại (UTC) - 5 phút
            return startTime.ToUniversalTime() > DateTime.UtcNow.AddMinutes(-5);
        })
        .WithMessage(c =>
        {
            // Logic thông báo: Hiển thị giờ Việt Nam cho người dùng dễ hiểu
            // Lấy giờ UTC hiện tại + 7 tiếng
            var nowVN = DateTime.UtcNow.AddHours(7);
            return $"Thời gian bắt đầu không được ở quá khứ (Phải sau {nowVN:HH:mm dd/MM/yyyy}).";
        });

        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");

        RuleFor(c => c.Description).NotEmpty().MaximumLength(1000);

        RuleForEach(c => c.EquipmentIds)
            .CustomAsync(async (eqId, context, token) =>
            {
                var cmd = (CreateEquipmentMaintainScheduleCommand)context.InstanceToValidate;
                var conflict = await repo.GetConflictInfoAsync(eqId, cmd.StartTime, cmd.EndTime, token);

                if (conflict != null)
                {
                    var startVN = conflict.StartTime.AddHours(7);
                    var endVN = conflict.EndTime.AddHours(7);

                    string timeString;

                    // Logic hiển thị thông minh
                    if (startVN.Date == endVN.Date)
                    {
                        // Nếu cùng ngày: "08:00 01/12 đến 10:00"
                        timeString = $"(Từ {startVN:HH:mm dd/MM} đến {endVN:HH:mm})";
                    }
                    else
                    {
                        // Nếu khác ngày: "08:00 01/12 đến 08:30 02/12" <--- SỬA CHỖ NÀY
                        timeString = $"(Từ {startVN:HH:mm dd/MM} đến {endVN:HH:mm dd/MM})";
                    }

                    var errorMessage = $"Thiết bị '{conflict.EquipmentName}' đang có lịch bảo trì tại '{conflict.LabRoomName}' {timeString}.";
                    context.AddFailure(errorMessage);
                }
            });
    }
}
