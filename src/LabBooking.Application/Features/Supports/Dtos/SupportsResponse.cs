namespace LabBooking.Application.Features.Supports.Dtos;

public record SupportsResponse(
    Guid Id,
    string Title,
    string Content,
    string Answer,
    Guid CreatedById
);
