namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public record CreateIncidentCommand(
    Guid FromRoomCheckId,
    IncidentType Type,
    LevelOfImportance ImportanceLevel,
    string Description,
    List<Guid>? EquipmentIds
) : IRequest<Guid>;