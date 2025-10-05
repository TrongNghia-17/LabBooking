namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public record GetAllIncidentsQuery() : IRequest<IEnumerable<GetAllIncidentsResponse>>;
