namespace LabBooking.Application.Features.EquipmentCategories.Commands.Create;

public class CreateEquipmentCategoryCommandValidator : AbstractValidator<CreateEquipmentCategoryCommand>
{
    public CreateEquipmentCategoryCommandValidator(IEquipmentCategoryRepository repository)
    {
        // 1. Validate Tên (Bắt buộc, độ dài)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên loại thiết bị không được để trống.")
            .MaximumLength(100).WithMessage("Tên loại thiết bị không được vượt quá 100 ký tự.");

        // 2. Validate Trùng Tên (Gọi xuống DB kiểm tra)
        RuleFor(x => x.Name)
            .MustAsync(async (name, token) =>
            {
                // Nếu tên rỗng thì bỏ qua (để rule NotEmpty bên trên bắt)
                if (string.IsNullOrWhiteSpace(name)) return true;

                bool exists = await repository.IsNameExistsAsync(name, token);
                return !exists; // Trả về true nếu KHÔNG tồn tại (hợp lệ)
            })
            .WithMessage("Tên loại thiết bị này đã tồn tại. Vui lòng chọn tên khác.");

        // 3. Validate Mô tả (Độ dài tùy chọn)
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
    }
}
