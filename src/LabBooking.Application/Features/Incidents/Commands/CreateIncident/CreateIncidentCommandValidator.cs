namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator(IEquipmentRepository equipmentRepository)
    {
        RuleFor(x => x.LabRoomId)
            .NotEmpty().WithMessage("Vui lòng chọn phòng Lab xảy ra sự cố.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại sự cố không hợp lệ.");


        RuleFor(x => x.ImportanceLevel)
            .IsInEnum().WithMessage("Mức độ quan trọng không hợp lệ.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Vui lòng nhập mô tả sự cố.")
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(x => x.EquipmentIds)
            .NotEmpty() // NotEmpty với List nghĩa là != null và Count > 0
            .When(x => x.Type == IncidentType.EquipmentFailure)
            .WithMessage("Vui lòng chọn ít nhất một thiết bị bị hỏng.");

        // 6. Nếu KHÔNG phải lỗi thiết bị -> Danh sách PHẢI rỗng hoặc Null
        RuleFor(x => x.EquipmentIds)
            .Must(ids => ids == null || ids.Count == 0)
            .When(x => x.Type != IncidentType.EquipmentFailure)
            .WithMessage("Không được chọn thiết bị nếu loại sự cố không phải là 'Hư hỏng thiết bị'.");

        // 7. Kiểm tra từng thiết bị trong danh sách có thuộc phòng Lab không
        // Sử dụng RuleForEach để duyệt qua từng phần tử trong List
        RuleForEach(x => x.EquipmentIds)
            .CustomAsync(async (equipmentId, context, token) =>
            {
                // Lấy command gốc để biết LabRoomId
                var command = (CreateIncidentCommand)context.InstanceToValidate;

                // Gọi Repo kiểm tra xem thiết bị này có nằm trong phòng Lab đang chọn không
                var isInLab = await equipmentRepository.IsEquipmentInLabAsync(equipmentId, command.LabRoomId, token);

                if (!isInLab)
                {
                    // Nếu sai thì bắn lỗi kèm ID thiết bị (hoặc bạn có thể query lấy tên thiết bị để báo lỗi đẹp hơn)
                    context.AddFailure("EquipmentIds", $"Thiết bị có ID {equipmentId} không thuộc phòng Lab này.");
                }
            })
            .When(x => x.Type == IncidentType.EquipmentFailure && x.EquipmentIds != null);
    }
}
