namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateStatus
{
    public class UpdateRoomMaintainStatusCommandValidator : AbstractValidator<UpdateRoomMaintainStatusCommand>
    {
        public UpdateRoomMaintainStatusCommandValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Trạng thái không hợp lệ.");
        }
    }
}
