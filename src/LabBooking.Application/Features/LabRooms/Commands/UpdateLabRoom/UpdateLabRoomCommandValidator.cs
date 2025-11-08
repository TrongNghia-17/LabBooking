using LabBooking.Application.Resources;
using Microsoft.Extensions.Localization;

namespace LabBooking.Application.Features.LabRooms.Commands.UpdateLabRoom;

public class UpdateLabRoomCommandValidator : AbstractValidator<UpdateLabRoomCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILabRoomRepository _labRoomRepository;
    private readonly IStringLocalizer<LabRoomMessages> _localizer;

    public UpdateLabRoomCommandValidator(
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
            .WithMessage(_localizer["LabNameMaxLength"]);

        RuleFor(c => c)
            .MustAsync(async (command, token) =>
                await BeUniqueLabName(command.Id, command.LabName, token))
            .WithMessage(_localizer["LabNameUnique"])
            .WithName("LabName");

        RuleFor(c => c.Location)
            .MaximumLength(200)
            .WithMessage(_localizer["LocationMaxLength"]);

        RuleFor(c => c.MaximumLimit)
            .GreaterThan(0)
            .When(c => c.MaximumLimit.HasValue)
            .WithMessage(_localizer["MaximumLimitGreaterThan"]);

        RuleFor(c => c.IsActive)
            .NotNull()
            .WithMessage(_localizer["IsActiveRequired"]);

        RuleFor(c => c.MainManagerId)
            .MustAsync(UserMustExist)
            .When(c => c.MainManagerId.HasValue)
            .WithMessage(_localizer["MainManagerNotFound"]);
    }

    /// <summary>
    /// Kiểm tra User (Manager) có tồn tại hay không
    /// </summary>
    private async Task<bool> UserMustExist(Guid? id, CancellationToken token)
    {
        if (id == null) return true;

        return await _userRepository.ExistsAsync(id.Value, token);
    }

    /// <summary>
    /// Kiểm tra LabName là duy nhất, ngoại trừ chính LabRoom đang được cập nhật
    /// </summary>
    private async Task<bool> BeUniqueLabName(Guid id, string labName, CancellationToken token)
    {
        return await _labRoomRepository.IsLabNameUniqueAsync(id, labName, token);
    }
}
