namespace LabBooking.Domain.NonEntities;

public class LabDailySchedule
{
    public Guid LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<LabSlotSchedule> Schedules { get; set; } = [];
}

public class LabSlotSchedule
{
    public string TimeRange { get; set; } = string.Empty;
    public string ActivityTitle { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
}