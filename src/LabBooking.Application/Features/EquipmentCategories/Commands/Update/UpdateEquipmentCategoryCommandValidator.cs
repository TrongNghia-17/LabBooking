namespace LabBooking.Application.Features.EquipmentCategories.Commands.Update;

public class UpdateEquipmentCategoryCommandValidator : AbstractValidator<UpdateEquipmentCategoryCommand>
{
    public UpdateEquipmentCategoryCommandValidator(IEquipmentCategoryRepository repository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID không hợp lệ.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên loại thiết bị không được để trống.")
            .MaximumLength(100).WithMessage("Tên loại thiết bị không được vượt quá 100 ký tự.");

        // Validate Check trùng tên (Trừ chính nó ra)
        RuleFor(x => x)
            .MustAsync(async (command, token) =>
            {
                if (string.IsNullOrWhiteSpace(command.Name)) return true;

                // Kiểm tra xem có ai KHÁC đang dùng tên này không
                bool exists = await repository.IsNameExistsExcludeIdAsync(command.Name, command.Id, token);
                return !exists;
            })
            .WithMessage("Tên loại thiết bị đã được sử dụng bởi một loại khác.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
    }
}
