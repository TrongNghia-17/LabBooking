using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public class CreateDoorRequestCommandValidator : AbstractValidator<CreateDoorRequestCommand>
{
    private readonly IDoorRequestRepository _doorRequestRepository;
    private readonly IBookingRepository _bookingRepository;

    public CreateDoorRequestCommandValidator(
        IDoorRequestRepository doorRequestRepository,
        IBookingRepository bookingRepository)
    {
        _doorRequestRepository = doorRequestRepository;
        _bookingRepository = bookingRepository;

        RuleFor(x => x.BookingCode)
            .NotEmpty().WithMessage("Mã đặt phòng không được để trống.");

        RuleFor(x => x.RequestDate)
            .NotEmpty().WithMessage("Vui lòng chọn ngày.");

        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("Vui lòng chọn ca học.");

        RuleFor(x => x)
            .MustAsync(ValidateDateAndSlot)
            .WithMessage("Ngày và ca học không thuộc booking này.")
            .MustAsync(NotHavePendingRequest)
            .WithMessage("Yêu cầu mở cửa cho ngày và ca này đang chờ duyệt. Vui lòng đợi.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Vui lòng nhập lý do mở cửa.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }

    // ✅ ĐÚNG: Nhận CancellationToken từ FluentValidation
    private async Task<bool> ValidateDateAndSlot(
        CreateDoorRequestCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByCodeAsync(
            command.BookingCode,
            cancellationToken
        );

        if (booking == null) return false;

        // Kiểm tra xem RequestDate và SlotId có trong booking không
        return booking.Slots.Any(bs =>
            bs.Date == command.RequestDate &&
            bs.SlotId == command.SlotId);
    }

    // ✅ ĐÚNG: Nhận CancellationToken
    private async Task<bool> NotHavePendingRequest(
        CreateDoorRequestCommand command,
        CancellationToken cancellationToken)
    {
        var hasPending = await _doorRequestRepository.HasPendingRequestAsync(
            command.BookingCode,
            command.RequestDate,
            command.SlotId
        );
        return !hasPending;
    }
}