using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Slots.Commands.UpdateSlot;

public class UpdateSlotCommand : IRequest
{
    [JsonIgnore] // Không cần gửi Id trong Body JSON, sẽ lấy từ URL
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotIndex { get; set; }
    public string Label { get; set; } = string.Empty;
}
