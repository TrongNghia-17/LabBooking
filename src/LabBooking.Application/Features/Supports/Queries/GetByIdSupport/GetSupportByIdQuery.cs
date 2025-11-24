using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetByIdSupport;

/// <summary>
/// Represents a query to retrieve a single support ticket by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the support ticket to retrieve.</param>
/// <remarks>
/// This query returns a <see cref="SupportsResponse"/> object.
/// </remarks>
public record GetSupportByIdQuery(Guid Id) : IRequest<SupportsResponse>;

