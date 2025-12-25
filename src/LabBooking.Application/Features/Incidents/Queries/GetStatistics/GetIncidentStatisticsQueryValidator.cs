namespace LabBooking.Application.Features.Incidents.Queries.GetStatistics;

public class GetIncidentStatisticsQueryValidator : AbstractValidator<GetIncidentStatisticsQuery>
{
    public GetIncidentStatisticsQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2020, DateTime.UtcNow.Year + 1)
            .WithMessage("Năm không hợp lệ.");
    }
}
