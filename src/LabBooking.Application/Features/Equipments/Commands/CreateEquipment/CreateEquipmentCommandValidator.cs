namespace LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(c => c.EquipmentName)
            .NotEmpty()
            .WithMessage("Equipment Name is required.")
            .MaximumLength(100)
            .WithMessage("Equipment Name cannot be longer than 100 characters.");

        RuleFor(c => c.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot be longer than 500 characters.");

        RuleFor(c => c.LabRoomId)
            .NotEmpty()
            .WithMessage("Lab Room ID is required.");

        RuleFor(c => c.Status)
            .Must(BeValidEquipmentStatus)
            .When(c => !string.IsNullOrEmpty(c.Status))
            .WithMessage($"'status' không hợp lệ. Phải là một trong các giá trị: {GetValidStatuses()}");
    }

    /// <summary>
    /// Kiểm tra xem chuỗi có thể được parse thành EquipmentStatus hay không
    /// (true = phân biệt chữ hoa/thường)
    /// </summary>
    private bool BeValidEquipmentStatus(string? status)
    {
        return Enum.TryParse<EquipmentStatus>(status, true, out _);
    }

    /// <summary>
    /// Lấy danh sách tên Enum hợp lệ để hiển thị trong thông báo lỗi
    /// </summary>
    private string GetValidStatuses()
    {
        return string.Join(", ", Enum.GetNames(typeof(EquipmentStatus)));
    }
}
