using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.UsagePolicies.Commands.UpdateUsagePolicy;

/// <summary>
/// Command để xử lý logic cập nhật một UsagePolicy.
/// </summary>
public record UpdateUsagePolicyCommand() : IRequest<Unit>
{
    /// <summary>
    /// ID của policy, được gán từ route.
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; }

    // Các trường có thể cập nhật,
    // dựa trên CreateUsagePolicyCommand và UsagePolicy.cs
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool ForAllLabRooms { get; set; }
    public Guid? LabRoomId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Trạng thái kích hoạt (tương tự LabRoom)
    /// </summary>
    public bool IsActive { get; set; }
}
