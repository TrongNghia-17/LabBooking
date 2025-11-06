using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

/// <summary>
/// Record chứa dữ liệu đầu vào để tạo một LabRoom mới.
/// </summary>
/// <param name="LabName">Tên của phòng lab.</param>
/// <param name="Location">Vị trí của phòng lab.</param>
/// <param name="MaximumLimit">Số lượng người tối đa.</param>
/// <param name="MainManagerId">ID người quản lý chính (nếu có).</param>
/// <param name="CreatedById">ID của người dùng tạo phòng lab.</param>
public record CreateLabRoomCommand(
    string LabName,
    string? Location,
    int? MaximumLimit,
    Guid? MainManagerId,
    Guid CreatedById
) : IRequest<Guid>;
