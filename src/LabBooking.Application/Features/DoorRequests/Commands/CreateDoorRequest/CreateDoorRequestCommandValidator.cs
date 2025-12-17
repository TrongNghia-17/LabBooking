namespace LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public class CreateDoorRequestCommandValidator : AbstractValidator<CreateDoorRequestCommand>
{
    private readonly IDoorRequestRepository _doorRequestRepository;

    public CreateDoorRequestCommandValidator(IDoorRequestRepository doorRequestRepository)
    {
        _doorRequestRepository = doorRequestRepository;

        RuleFor(x => x.BookingCode)
            .NotEmpty().WithMessage("Mã đặt phòng không được để trống.")
            .MustAsync(NotHavePendingRequest).WithMessage("Yêu cầu mở cửa cho mã đặt phòng này đang chờ duyệt. Vui lòng đợi.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Vui lòng nhập lý do mở cửa.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }

    // Custom Validate: Chặn Spam
    private async Task<bool> NotHavePendingRequest(string bookingCode, CancellationToken token)
    {
        var hasPending = await _doorRequestRepository.HasPendingRequestAsync(bookingCode);
        return !hasPending; // Hợp lệ nếu KHÔNG có pending request
    }
}
