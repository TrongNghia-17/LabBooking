namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentStatisticResponse
{
    public int MonthNumber { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int IncidentCount { get; set; }
}
