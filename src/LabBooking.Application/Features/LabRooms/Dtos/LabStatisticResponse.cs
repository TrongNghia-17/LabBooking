namespace LabBooking.Application.Features.LabRooms.Dtos;

public class LabStatisticResponse
{
    public Guid LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public List<MonthlyStatistic> MonthlyData { get; set; } = new();
}

public class MonthlyStatistic
{
    public int Month { get; set; }
    public int UsageCount { get; set; }      // Số lượt đặt phòng sử dụng
    public int MaintenanceCount { get; set; } // Số lượt bảo trì
}
