namespace LabBooking.Application.Features.Incidents.Commands.Delete;

public record DeleteIncidentCommand(Guid Id) : IRequest<bool>;
