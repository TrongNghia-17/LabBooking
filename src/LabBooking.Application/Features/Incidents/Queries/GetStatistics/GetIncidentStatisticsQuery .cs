namespace LabBooking.Application.Features.Incidents.Queries.GetStatistics;

public class GetIncidentStatisticsQuery : IRequest<IEnumerable<IncidentStatisticResponse>>
{
    public int Year { get; set; }
}