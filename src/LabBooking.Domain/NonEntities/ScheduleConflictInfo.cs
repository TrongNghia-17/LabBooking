namespace LabBooking.Domain.NonEntities;

public class ScheduleConflictInfo
{
    public string EquipmentName { get; set; } = string.Empty;
    public string LabRoomName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
