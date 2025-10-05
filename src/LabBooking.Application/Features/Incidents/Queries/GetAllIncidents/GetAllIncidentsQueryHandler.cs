namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetAllIncidentsQueryHandler(
    ILogger<GetAllIncidentsQueryHandler> logger,
    IIncidentRepository incidentRepository,
    IMapper mapper) : IRequestHandler<GetAllIncidentsQuery, IEnumerable<GetAllIncidentsResponse>>
{
    public async Task<IEnumerable<GetAllIncidentsResponse>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all incidents");
        var incidents = await incidentRepository.GetAllAsync();

        var incidentResponses = mapper.Map<IEnumerable<GetAllIncidentsResponse>>(incidents);
        return incidentResponses;
    }
}
