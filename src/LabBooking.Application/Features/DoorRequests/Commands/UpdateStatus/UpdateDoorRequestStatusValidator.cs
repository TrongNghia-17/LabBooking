namespace LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;

public class UpdateDoorRequestStatusValidator : AbstractValidator<UpdateDoorRequestStatusCommand>
{
    public UpdateDoorRequestStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mã yêu cầu không được để trống.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ.");

        RuleFor(x => x.Note)
            .NotEmpty()
            .When(x => x.NewStatus == DoorRequestStatus.Accepted || x.NewStatus == DoorRequestStatus.Rejected)
            .WithMessage("Bạn phải nhập phản hồi/lý do cho sinh viên khi Chấp nhận hoặc Từ chối.");
    }
}
