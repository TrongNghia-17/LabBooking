namespace LabBooking.Application.Features.DoorRequests.Commands.Create;

public class CreateDoorRequestValidator : AbstractValidator<CreateDoorRequestCommand>
{
    public CreateDoorRequestValidator()
    {
        RuleFor(x => x.LabRoomId).NotEmpty().WithMessage("Vui lòng chọn phòng Lab.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Loại yêu cầu không hợp lệ.");
    }
}
