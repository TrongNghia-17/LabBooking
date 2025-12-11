using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

public class GetAllMaintainSchedulesQuery : IRequest<PagedResult<EquipmentMaintainScheduleResponse>>
{
    // 1. Filter (Lọc)
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public MaintenanceStatus? Status { get; set; }

    // 2. Pagination (Phân trang) - THÊM MỚI
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    // 3. Sort (Sắp xếp)
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = true;
}
