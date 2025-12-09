using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

public class GetAllMaintainSchedulesQuery : IRequest<IEnumerable<EquipmentMaintainScheduleResponse>>
{
    // 1. Filter (Lọc)
    public DateTime? FromDate { get; set; } // Từ ngày
    public DateTime? ToDate { get; set; }   // Đến ngày
    public MaintenanceStatus? Status { get; set; } // Trạng thái (NotYet/Done)

    // 2. Sort (Sắp xếp)
    public string? SortBy { get; set; } // "Date" hoặc "Status"
    public bool IsDescending { get; set; } = true; // Mặc định là giảm dần (Mới nhất lên đầu)
}
