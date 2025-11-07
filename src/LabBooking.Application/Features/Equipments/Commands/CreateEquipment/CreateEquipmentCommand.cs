namespace LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

/// <summary>
/// Record chứa dữ liệu đầu vào để tạo một Equipment mới.
/// </summary>
/// <param name="EquipmentName">Tên của thiết bị.</param>
/// <param name="Description">Mô tả thiết bị.</param>
/// <param name="LabRoomId">ID của phòng lab chứa thiết bị này.</param>
/// <param name="Status">Trạng thái của thiết bị (mặc định là Available nếu null).</param>
/// <param name="IsAvailable">Thiết bị có sẵn sàng (mặc định là true nếu null).</param>
public record CreateEquipmentCommand(
    string EquipmentName,
    string? Description,
    Guid LabRoomId,
    string Status,
    bool IsAvailable
) : IRequest<Guid>;
