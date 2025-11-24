namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public record CreateIncidentCommand(
    Guid LabRoomId,
    Guid SlotId,
    IncidentType Type,
    string Description,
    LevelOfImportance ImportanceLevel
) : IRequest<Guid>;
