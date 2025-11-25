namespace LabBooking.Application.Features.Supports.Dtos;

/// <summary>
/// Represents the data transfer object for a support ticket.
/// </summary>
/// <param name="Id">The unique identifier of the support ticket.</param>
/// <param name="Title">The title or subject of the support ticket.</param>
/// <param name="Content">The main content or description of the issue.</param>
/// <param name="Answer">The (optional) answer or resolution provided for the ticket.</param>
/// <param name="CreatedById">The identifier of the user who created the ticket.</param>
public record SupportsResponse(
    Guid Id,
    string Title,
    string Content,
    string? Answer,
    Guid CreatedById,
    string Status,
    DateTime CreatedAt,
    DateTime? RespondedAt
);
