using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetAllRoomMaintainSchedules
{
    /// <summary>
    /// Record chứa các tham số để truy vấn danh sách lịch bảo trì.
    /// </summary>
    public record GetAllRoomMaintainSchedulesQuery(
        // Tham số tìm kiếm
        string? SearchPhrase,
        DateTime? From,
        DateTime? To,
        // Bỏ LabRoomId

        // Tham số lọc theo trạng thái
        RoomMaintainStatus? Status,

        // Tham số phân trang
        int PageNumber,
        int PageSize,

        // Tham số sắp xếp
        string? SortBy,
        SortDirection SortDirection
    ) : IRequest<PagedResult<RoomMaintainScheduleResponse>>;
}
