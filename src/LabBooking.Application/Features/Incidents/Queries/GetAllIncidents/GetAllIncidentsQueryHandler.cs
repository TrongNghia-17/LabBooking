namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetAllIncidentsQueryHandler(
    ILogger<GetAllIncidentsQueryHandler> logger,
    IIncidentRepository incidentRepository,
    IMapper mapper) : IRequestHandler<GetAllIncidentsQuery, PagedResult<IncidentsResponse>>
{
    public async Task<PagedResult<IncidentsResponse>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all incidents");
        var (incidents, totalCount) = await incidentRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var incidentsResponse = mapper.Map<IEnumerable<IncidentsResponse>>(incidents);

        var result = new PagedResult<IncidentsResponse>(
            incidentsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);
        return result;
    }
}
