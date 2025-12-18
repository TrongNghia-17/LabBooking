namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    // Inject thêm IRoomCheckRepository để tra cứu phòng Lab
    public CreateIncidentCommandValidator(
        IEquipmentRepository equipmentRepository,
        IRoomCheckRepository roomCheckRepository)
    {
        RuleFor(x => x.FromRoomCheckId)
            .NotEmpty().WithMessage("Không xác định được đợt kiểm tra (FromRoomCheckId).");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại sự cố không hợp lệ.");

        RuleFor(x => x.ImportanceLevel)
            .IsInEnum().WithMessage("Mức độ quan trọng không hợp lệ.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Vui lòng nhập mô tả sự cố.")
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        // Rule cho Thiết bị: Nếu là lỗi thiết bị thì phải chọn
        RuleFor(x => x.EquipmentIds)
            .NotEmpty()
            .When(x => x.Type == IncidentType.EquipmentFailure)
            .WithMessage("Vui lòng chọn ít nhất một thiết bị bị hỏng.");

        // Rule ngược lại: Nếu không phải lỗi thiết bị thì không được gửi list thiết bị (để tránh rác data)
        RuleFor(x => x.EquipmentIds)
            .Must(ids => ids == null || ids.Count == 0)
            .When(x => x.Type != IncidentType.EquipmentFailure)
            .WithMessage("Không được chọn thiết bị nếu loại sự cố không phải là 'Hư hỏng thiết bị'.");

        // --- VALIDATION NÂNG CAO (LOGIC DATABASE) ---
        // Sử dụng CustomAsync ở cấp độ Class (RuleFor(x => x)) để xử lý logic phụ thuộc lẫn nhau
        RuleFor(x => x)
            .CustomAsync(async (command, context, token) =>
            {
                // Chỉ chạy logic này nếu là lỗi thiết bị và có danh sách thiết bị
                if (command.Type == IncidentType.EquipmentFailure && command.EquipmentIds != null && command.EquipmentIds.Any())
                {
                    // 1. Lấy thông tin RoomCheck để biết LabRoomId là gì
                    // (Hàm GetByIdWithLabRoomAsync chúng ta đã tạo trong Repo ở bước trước)
                    var roomCheck = await roomCheckRepository.GetByIdWithLabRoomAsync(command.FromRoomCheckId, token);

                    if (roomCheck == null)
                    {
                        context.AddFailure("FromRoomCheckId", "Không tìm thấy thông tin phiếu kiểm tra trong hệ thống.");
                        return; // Dừng check tiếp nếu không thấy phiếu
                    }

                    var labId = roomCheck.LabRoomId;

                    // 2. Duyệt qua từng thiết bị để kiểm tra xem có thuộc phòng Lab này không
                    foreach (var eqId in command.EquipmentIds)
                    {
                        var isInLab = await equipmentRepository.IsEquipmentInLabAsync(eqId, labId, token);

                        if (!isInLab)
                        {
                            context.AddFailure("EquipmentIds", $"Thiết bị (ID: {eqId}) không thuộc phòng {roomCheck.LabRoom?.LabName ?? "này"}.");
                        }
                    }
                }
            });
    }
}