using LabBooking.Application.Resources;
using Microsoft.Extensions.Localization;

namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

public class CreateLabRoomCommandValidator
    : AbstractValidator<CreateLabRoomCommand>
{

    private readonly IUserRepository _userRepository;
    private readonly ILabRoomRepository _labRoomRepository;
    private readonly IStringLocalizer<LabRoomMessages> _localizer;

    public CreateLabRoomCommandValidator(
        IUserRepository userRepository,
        ILabRoomRepository labRoomRepository,
        IStringLocalizer<LabRoomMessages> localizer)
    {
        _userRepository = userRepository;
        _labRoomRepository = labRoomRepository;
        _localizer = localizer;

        RuleFor(c => c.LabName)
            .NotEmpty()
            .WithMessage(_localizer["LabNameRequired"])
            .MaximumLength(100)
            .WithMessage(_localizer["LabNameMaxLength"])
            .MustAsync(BeUniqueLabName)
            .WithMessage(_localizer["LabNameUnique"]);

        RuleFor(c => c.Location)
            .MaximumLength(200)
            .WithMessage(_localizer["LocationMaxLength"]);

        RuleFor(c => c.MaximumLimit)
            .GreaterThan(0)
            .When(c => c.MaximumLimit.HasValue)
            .WithMessage(_localizer["MaximumLimitGreaterThan"]);

        RuleFor(c => c.MainManagerId)
            .MustAsync(UserMustExist)
            .When(c => c.MainManagerId.HasValue)
            .WithMessage(_localizer["MainManagerNotFound"]);
    }

    private async Task<bool> UserMustExist(Guid? id, CancellationToken token)
    {
        if (id == null) return true;

        return await _userRepository.ExistsAsync(id.Value, token);
    }

    private async Task<bool> BeUniqueLabName(string labName, CancellationToken token)
    {
        return await _labRoomRepository.IsLabNameUniqueAsync(labName, token);
    }

}

