namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public class CreateRoomCheckCommandValidator : AbstractValidator<CreateRoomCheckCommand>
{
    public CreateRoomCheckCommandValidator()
    {
        RuleFor(x => x.LabRoomId)
            .NotEmpty().WithMessage("Vui lòng chọn phòng Lab.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại kiểm tra không hợp lệ.");

        RuleForEach(x => x.EquipmentDetails).ChildRules(item =>
        {
            item.RuleFor(x => x.EquipmentId)
                .NotEmpty();

            item.RuleFor(x => x.IssueDescription)
                .NotEmpty()
                .When(x => x.IsOK == false)
                .WithMessage("Vui lòng nhập mô tả lỗi cho thiết bị hỏng.");
        });
    }
}