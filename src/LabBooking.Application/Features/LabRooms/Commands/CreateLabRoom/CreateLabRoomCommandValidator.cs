namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

public class CreateLabRoomCommandValidator
    : AbstractValidator<CreateLabRoomCommand>
{

    private readonly IUserRepository _userRepository;
    private readonly ILabRoomRepository _labRoomRepository;

    public CreateLabRoomCommandValidator(
        IUserRepository userRepository,
        ILabRoomRepository labRoomRepository)
    {
        _userRepository = userRepository;
        _labRoomRepository = labRoomRepository;

        RuleFor(c => c.LabName)
            .NotEmpty()
            .WithMessage("Lab Name is required.")
            .MaximumLength(100)
            .WithMessage("Lab Name cannot be longer than 100 characters.")
            .MustAsync(BeUniqueLabName)
            .WithMessage("Lab Name đã tồn tại.");

        RuleFor(c => c.Location)
            .MaximumLength(200)
            .WithMessage("Location cannot be longer than 200 characters.");

        RuleFor(c => c.MaximumLimit)
            .GreaterThan(0)
            .When(c => c.MaximumLimit.HasValue)
            .WithMessage("Maximum Limit must be greater than 0.");

        RuleFor(c => c.MainManagerId)
            .MustAsync(UserMustExist)
            .When(c => c.MainManagerId.HasValue)
            .WithMessage("Người quản lý (MainManagerId) không tồn tại.");
    }

    private async Task<bool> UserMustExist(Guid? id, CancellationToken token)
    {
        if (id == null) return true;

        return await _userRepository.ExistsAsync(id.Value);
    }

    private async Task<bool> BeUniqueLabName(string labName, CancellationToken token)
    {
        return await _labRoomRepository.IsLabNameUniqueAsync(labName);
    }

}

