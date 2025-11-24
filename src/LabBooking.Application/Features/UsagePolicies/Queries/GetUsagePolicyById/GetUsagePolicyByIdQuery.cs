using LabBooking.Application.Features.UsagePolicies.Dtos;

namespace LabBooking.Application.Features.UsagePolicies.Queries.GetUsagePolicyById;

/// <summary>
/// Record chứa ID để truy vấn một UsagePolicy.
/// </summary>
public record GetUsagePolicyByIdQuery(Guid Id) : IRequest<UsagePolicyResponse>;
