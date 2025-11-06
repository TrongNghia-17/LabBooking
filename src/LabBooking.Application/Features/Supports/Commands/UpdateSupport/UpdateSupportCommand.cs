using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

/// <summary>
/// Command để xử lý logic cập nhật một support ticket.
/// </summary>
public record UpdateSupportCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Answer { get; set; } = default!;
}


