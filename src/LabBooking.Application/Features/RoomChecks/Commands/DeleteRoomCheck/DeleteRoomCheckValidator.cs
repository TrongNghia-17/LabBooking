namespace LabBooking.Application.Features.RoomChecks.Commands.DeleteRoomCheck;

public class DeleteRoomCheckValidator : AbstractValidator<DeleteRoomCheckCommand>
{
    public DeleteRoomCheckValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID phiếu kiểm tra không được để trống.");
    }
}