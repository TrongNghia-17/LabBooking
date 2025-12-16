namespace LabBooking.Domain.NonEntities;

// Class này nằm ở Domain, nên Interface Repository có thể dùng nó thoải mái
public class LabAvailabilityModel
{
    public Guid LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // Danh sách các Slot còn trống (Dạng Model hoặc Entity con)
    public List<SlotTimeModel> AvailableSlots { get; set; } = new();
}

public class SlotTimeModel
{
    public Guid Id { get; set; }
    public int SlotIndex { get; set; }
    public TimeOnly StartTime { get; set; } // Dùng TimeOnly của Domain
    public TimeOnly EndTime { get; set; }
}
