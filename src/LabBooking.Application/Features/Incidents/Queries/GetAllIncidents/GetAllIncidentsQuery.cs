namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public record GetAllIncidentsQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
    ) : IRequest<PagedResult<GetAllIncidentsResponse>>;
