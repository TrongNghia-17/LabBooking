namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

/// <summary>
/// Defines the validation rules for the <see cref="UpdateSupportCommand"/>.
/// </summary>
public class UpdateSupportCommandValidator : AbstractValidator<UpdateSupportCommand>
{
    public UpdateSupportCommandValidator()
    {
        RuleFor(c => c.Answer)
            .NotEmpty()
            .WithMessage("Phải nhập câu trả lời khi trạng thái là Đã phản hồi.")
            .When(c => c.Status == SupportStatus.Responded);

        RuleFor(c => c.Status)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ.")

            // Bắt buộc trạng thái mới KHÔNG ĐƯỢC là Pending
            .NotEqual(SupportStatus.Pending)
            .WithMessage("Đã xử lý thì không được để trạng thái là 'Chờ xử lý' (Pending). Vui lòng chọn 'Đã phản hồi' hoặc 'Bỏ qua'.");
    }
}
