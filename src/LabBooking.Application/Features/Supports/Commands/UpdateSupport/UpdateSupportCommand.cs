using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

/// <summary>
/// Represents the command used to update an existing support ticket.
/// </summary>
public record UpdateSupportCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Answer { get; set; } = default!;
}


