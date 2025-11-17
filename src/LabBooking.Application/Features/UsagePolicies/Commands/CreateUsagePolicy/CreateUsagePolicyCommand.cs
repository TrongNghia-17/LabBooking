namespace LabBooking.Application.Features.UsagePolicies.Commands.CreateUsagePolicy;

/// <summary>
/// Represents the command to create a new usage policy.
/// </summary>
public record CreateUsagePolicyCommand(
    string Title,
    string? Description,

    /// <summary>
    /// If true, this policy applies to all lab rooms.
    /// If false, LabRoomId must be provided.
    /// </summary>
    bool ForAllLabRooms,

    /// <summary>
    /// The specific LabRoom ID this policy applies to.
    /// Must be null if ForAllLabRooms is true.
    /// Must not be null if ForAllLabRooms is false.
    /// </summary>
    Guid? LabRoomId,

    DateTime? EffectiveFrom,
    DateTime? ExpirationDate
) : IRequest<Guid>;
