using LabBooking.Application.Features.UsagePolicies.Dtos;

namespace LabBooking.Application.Features.UsagePolicies.Queries.GetAllUsagePolicies;

public class GetAllUsagePoliciesQueryHandler(
    ILogger<GetAllUsagePoliciesQueryHandler> logger,
    IUsagePolicyRepository usagePolicyRepository, // Sử dụng IUsagePolicyRepository
    IMapper mapper
) : IRequestHandler<GetAllUsagePoliciesQuery, PagedResult<UsagePolicyResponse>>
{
    public async Task<PagedResult<UsagePolicyResponse>> Handle(GetAllUsagePoliciesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy danh sách usage policies...");

        // 1. Gọi Repository với tham số IsActive mới
        var (policies, totalCount) = await usagePolicyRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.IsActive, // Tham số lọc mới
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        // 2. Map từ List<UsagePolicy> (entity) sang List<UsagePolicyResponse> (DTO)
        var policiesResponse = mapper.Map<IEnumerable<UsagePolicyResponse>>(policies);

        // 3. Đóng gói kết quả PagedResult
        var result = new PagedResult<UsagePolicyResponse>(
            policiesResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
