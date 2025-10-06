namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandHandler(
    ILogger<CreateIncidentCommandHandler> logger,
    IMapper mapper,
    IIncidentRepository incidentRepository
    ) : IRequestHandler<CreateIncidentCommand, IncidentsResponse>
{
    public async Task<IncidentsResponse> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new incident");
        var incident = mapper.Map<Incident>(request);

        var createdIncident = await incidentRepository.Create(incident);

        var response = mapper.Map<IncidentsResponse>(createdIncident);
        return response;
    }
}
