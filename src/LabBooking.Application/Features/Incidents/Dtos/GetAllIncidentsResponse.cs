namespace LabBooking.Application.Features.Incidents.Dtos;

public record GetAllIncidentsResponse(
    Guid Id,
    Guid LabRoomId,
    Guid ReportedById,
    IncidentType Type,
    string Description,
    bool IsResolved,
    DateTime CreatedAt
);
