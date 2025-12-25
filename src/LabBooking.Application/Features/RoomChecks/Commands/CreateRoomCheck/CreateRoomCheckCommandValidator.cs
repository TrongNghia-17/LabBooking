namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public class CreateRoomCheckValidator : AbstractValidator<CreateRoomCheckCommand>
{
    public CreateRoomCheckValidator()
    {
        RuleFor(x => x.LabRoomId)
            .NotEmpty().WithMessage("Vui lòng chọn phòng Lab.");

        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("Vui lòng chọn Slot (Ca làm việc).");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại kiểm tra không hợp lệ.");

        RuleFor(x => x.Note)
            .NotEmpty()
            .When(x => !x.IsPassed)
            .WithMessage("Vui lòng nhập ghi chú khi kiểm tra Không Đạt.");
    }
}