namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetIncidentsQuery : IRequest<IEnumerable<IncidentResponse>>
{
    public string? SearchPhrase { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsResolved { get; set; }
    public LevelOfImportance? Importance { get; set; }
    public Guid? LabRoomId { get; set; }
    public bool IsDescending { get; set; } = true;
}
