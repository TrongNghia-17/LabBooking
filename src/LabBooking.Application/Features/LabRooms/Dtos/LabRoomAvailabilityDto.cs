namespace LabBooking.Application.Features.LabRooms.Dtos;

// LabBooking.Application.Features.LabRooms.Dtos.LabRoomAvailabilityDto.cs
public class LabRoomAvailabilityDto
{
    public Guid LabId { get; set; }
    public string LabName { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }

    // Danh sách các Slot ID còn trống trong ngày hôm đó
    public List<Guid> AvailableSlotIds { get; set; } = new();

    // (Optional) Danh sách chi tiết Slot để hiển thị (VD: "07:00 - 09:00")
    public List<SlotDto> AvailableSlots { get; set; } = new();
}

// LabBooking.Application.Features.Slots.Dtos.SlotDto.cs (Nếu chưa có)
public class SlotDto
{
    public Guid Id { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int SlotIndex { get; set; }
}
