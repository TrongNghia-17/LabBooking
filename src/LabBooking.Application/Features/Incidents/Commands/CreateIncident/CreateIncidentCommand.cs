namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public record CreateIncidentCommand(
    Guid? FromRoomCheckId,
    Guid? LabRoomId,
    IncidentType Type,
    LevelOfImportance ImportanceLevel,
    string Description,
    List<Guid>? EquipmentIds
) : IRequest<Guid>;