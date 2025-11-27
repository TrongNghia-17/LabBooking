namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.AutoUpdateStatus;

/// <summary>
/// Command kích hoạt việc kiểm tra và tự động cập nhật lịch bảo trì đã hết hạn sang trạng thái Done.
/// </summary>
public record AutoUpdateRoomMaintainStatusCommand() : IRequest<int>; // Trả về số lượng lịch được cập nhật
