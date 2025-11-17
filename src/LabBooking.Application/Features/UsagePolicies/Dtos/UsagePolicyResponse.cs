namespace LabBooking.Application.Features.UsagePolicies.Dtos;

/// <summary>
/// DTO đại diện cho một Usage Policy trả về cho client.
/// </summary>
public record UsagePolicyResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public bool ForAllLabRooms { get; init; }
    public Guid? LabRoomId { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
}
