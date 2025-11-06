using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetByIdSupport;

public record GetSupportByIdQuery(Guid Id) : IRequest<SupportsResponse>;

