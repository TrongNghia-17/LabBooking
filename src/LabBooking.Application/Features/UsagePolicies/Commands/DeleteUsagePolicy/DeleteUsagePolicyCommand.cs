namespace LabBooking.Application.Features.UsagePolicies.Commands.DeleteUsagePolicy;

/// <summary>
/// Command để xử lý logic xóa một UsagePolicy.
/// Sử dụng IRequest<Unit> vì không cần trả về dữ liệu gì sau khi xóa.
/// </summary>
public record DeleteUsagePolicyCommand(Guid Id) : IRequest<Unit>;
