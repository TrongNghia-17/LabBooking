using Microsoft.Extensions.Localization;

namespace LabBooking.Application.Features.UsagePolicies.Commands.UpdateUsagePolicy;

public class UpdateUsagePolicyCommandValidator : AbstractValidator<UpdateUsagePolicyCommand>
{
    private readonly ILabRoomRepository _labRoomRepository;
    private readonly IUsagePolicyRepository _usagePolicyRepository;
    // private readonly IStringLocalizer<UsagePolicyMessages> _localizer; // Bỏ

    public UpdateUsagePolicyCommandValidator(
        ILabRoomRepository labRoomRepository,
        IUsagePolicyRepository usagePolicyRepository
    // IStringLocalizer<UsagePolicyMessages> localizer // Bỏ
    )
    {
        _labRoomRepository = labRoomRepository;
        _usagePolicyRepository = usagePolicyRepository;
        // _localizer = localizer; // Bỏ

        // --- Title Rules ---
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(c => c)
            .MustAsync(async (command, token) =>
                await BeUniqueTitle(command.Id, command.Title, token))
            .WithMessage("A policy with this title already exists.")
            .WithName("Title");

        // --- Description Rule ---
        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        // --- IsActive Rule ---
        RuleFor(c => c.IsActive)
            .NotNull()
            .WithMessage("The 'IsActive' field is required.");

        // --- Conditional Logic for LabRoomId ---
        RuleFor(c => c.LabRoomId)
            .NotEmpty()
            .WithMessage("LabRoomId is required when 'ForAllLabRooms' is false.")
            .When(c => !c.ForAllLabRooms);

        RuleFor(c => c.LabRoomId)
            .Empty()
            .WithMessage("LabRoomId must be null when 'ForAllLabRooms' is true.")
            .When(c => c.ForAllLabRooms);

        // --- Kiểm tra sự tồn tại của LabRoomId ---
        RuleFor(c => c.LabRoomId)
            .MustAsync(LabRoomMustExist)
            .When(c => c.LabRoomId.HasValue && !c.ForAllLabRooms)
            .WithMessage("The specified LabRoom was not found.");

        // --- Date Logic ---
        RuleFor(c => c.ExpirationDate)
            .GreaterThan(c => c.EffectiveFrom)
            .WithMessage("Expiration Date must be after the Effective From date.")
            .When(c => c.ExpirationDate.HasValue && c.EffectiveFrom.HasValue);
    }

    /// <summary>
    /// (Giả định) Kiểm tra LabRoom có tồn tại hay không.
    /// </summary>
    private async Task<bool> LabRoomMustExist(Guid? id, CancellationToken token)
    {
        if (id == null) return true;
        // Giả định ILabRoomRepository có phương thức ExistsAsync
        return await _labRoomRepository.ExistsAsync(id.Value, token);
    }

    /// <summary>
    /// (Giả định) Kiểm tra Title là duy nhất, ngoại trừ chính Policy đang được cập nhật.
    /// </summary>
    private async Task<bool> BeUniqueTitle(Guid id, string title, CancellationToken token)
    {
        // Giả định IUsagePolicyRepository có phương thức tương tự IsLabNameUniqueAsync
        return await _usagePolicyRepository.IsTitleUniqueAsync(id, title, token);
    }
}
