namespace LabBooking.Application.Features.Incidents.Dtos;

public record IncidentsResponse(
    Guid Id,
    Guid LabRoomId,
    Guid ReportedById,
    IncidentType Type,
    string Description,
    bool IsResolved,
    DateTime CreatedAt
);
