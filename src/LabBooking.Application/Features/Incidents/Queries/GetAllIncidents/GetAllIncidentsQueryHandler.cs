namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetAllIncidentsQueryHandler(
    ILogger<GetAllIncidentsQueryHandler> logger,
    IIncidentRepository incidentRepository,
    IMapper mapper) : IRequestHandler<GetAllIncidentsQuery, PagedResult<GetAllIncidentsResponse>>
{
    public async Task<PagedResult<GetAllIncidentsResponse>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all incidents");
        var (incidents, totalCount) = await incidentRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var incidentResponses = mapper.Map<IEnumerable<GetAllIncidentsResponse>>(incidents);

        var result = new PagedResult<GetAllIncidentsResponse>(
            incidentResponses,
            totalCount,
            request.PageSize,
            request.PageNumber);
        return result;
    }
}
