using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAllEquipmentMaintainSchedules;

/// <summary>
/// Record chứa các tham số để truy vấn danh sách lịch bảo trì thiết bị.
/// </summary>
public record GetAllEquipmentMaintainSchedulesQuery(
    // Tham số tìm kiếm
    string? SearchPhrase,

    // (Đã loại bỏ EquipmentId)

    // Tham số lọc theo trạng thái
    EquimentpMaintainStatus? Status,

    // Tham số phân trang
    int PageNumber,
    int PageSize,

    // Tham số sắp xếp
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<EquipmentMaintainScheduleResponse>>;
