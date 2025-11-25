using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetByIdSupport;

/// <summary>
/// Represents a query to retrieve ALL support tickets created by the current authenticated user.
/// </summary>
// Không cần tham số đầu vào (như Id)
public record GetMySupportsQuery() : IRequest<IEnumerable<SupportsResponse>>;

