namespace LabBooking.Application.Features.Supports.Dtos;

/// <summary>
/// Represents the data transfer object for a support ticket.
/// </summary>
/// <param name="Id">The unique identifier of the support ticket.</param>
/// <param name="Title">The title or subject of the support ticket.</param>
/// <param name="Content">The main content or description of the issue.</param>
/// <param name="Answer">The (optional) answer or resolution provided for the ticket.</param>
/// <param name="CreatedById">The identifier of the user who created the ticket.</param>
public class SupportsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public Guid CreatedById { get; set; }

    // Các trường tùy biến (AutoMapper sẽ fill vào đây nhờ ForMember)
    public string? CreatedByName { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
