using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

/// <summary>
/// Represents the command containing the data required to create a new support ticket.
/// </summary>
/// <param name="Title">The title of the support ticket.</param>
/// <param name="Content">The detailed description or content of the support ticket.</param>
public record CreateSupportCommand(
    string Title,
    string Content
) : IRequest<Guid>;
