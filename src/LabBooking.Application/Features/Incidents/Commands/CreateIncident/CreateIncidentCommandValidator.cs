namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator(
        IEquipmentRepository equipmentRepository,
        IRoomCheckRepository roomCheckRepository,
        ILabRoomRepository labRoomRepository) // Thêm repo này để check LabRoom tồn tại
    {
        // --- QUY TẮC MỚI: HOẶC/HOẶC ---
        RuleFor(x => x)
            .Custom((command, context) =>
            {
                if (command.FromRoomCheckId.HasValue && command.LabRoomId.HasValue)
                {
                    context.AddFailure("Không thể cung cấp đồng thời FromRoomCheckId và LabRoomId.");
                }

                if (!command.FromRoomCheckId.HasValue && !command.LabRoomId.HasValue)
                {
                    context.AddFailure("Vui lòng cung cấp FromRoomCheckId (nếu từ phiếu kiểm tra) hoặc LabRoomId (nếu báo cáo độc lập).");
                }
            });

        // --- CÁC QUY TẮC CŨ KHÔNG ĐỔI ---
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.ImportanceLevel).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);

        RuleFor(x => x.EquipmentIds)
            .NotEmpty()
            .When(x => x.Type == IncidentType.EquipmentFailure);

        RuleFor(x => x.EquipmentIds)
            .Must(ids => ids == null || ids.Count == 0)
            .When(x => x.Type != IncidentType.EquipmentFailure);

        // --- VALIDATION NÂNG CAO (ĐÃ CẬP NHẬT) ---
        RuleFor(x => x)
            .CustomAsync(async (command, context, token) =>
            {
                if (command.Type != IncidentType.EquipmentFailure || command.EquipmentIds == null || !command.EquipmentIds.Any())
                {
                    return; // Bỏ qua nếu không phải lỗi thiết bị
                }

                Guid? targetLabId = null;
                string? labName = null;

                // Lấy LabId từ một trong hai nguồn
                if (command.FromRoomCheckId.HasValue)
                {
                    var roomCheck = await roomCheckRepository.GetByIdWithLabRoomAsync(command.FromRoomCheckId.Value, token);
                    if (roomCheck == null)
                    {
                        context.AddFailure("FromRoomCheckId", "Không tìm thấy thông tin phiếu kiểm tra.");
                        return;
                    }
                    targetLabId = roomCheck.LabRoomId;
                    labName = roomCheck.LabRoom?.LabName;
                }
                else if (command.LabRoomId.HasValue)
                {
                    var labRoom = await labRoomRepository.GetByIdAsync(command.LabRoomId.Value, token);
                    if (labRoom == null)
                    {
                        context.AddFailure("LabRoomId", "Không tìm thấy phòng Lab.");
                        return;
                    }
                    targetLabId = labRoom.Id;
                    labName = labRoom.LabName;
                }

                if (!targetLabId.HasValue) return;

                // Kiểm tra từng thiết bị
                foreach (var eqId in command.EquipmentIds)
                {
                    var isInLab = await equipmentRepository.IsEquipmentInLabAsync(eqId, targetLabId.Value, token);
                    if (!isInLab)
                    {
                        context.AddFailure("EquipmentIds", $"Thiết bị (ID: {eqId}) không thuộc phòng {labName ?? "này"}.");
                    }
                }
            });
    }
}