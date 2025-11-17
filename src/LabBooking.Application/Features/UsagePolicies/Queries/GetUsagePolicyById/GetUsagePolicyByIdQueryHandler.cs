using LabBooking.Application.Features.UsagePolicies.Dtos;

namespace LabBooking.Application.Features.UsagePolicies.Queries.GetUsagePolicyById;

public class GetUsagePolicyByIdQueryHandler(
    ILogger<GetUsagePolicyByIdQueryHandler> logger,
    IUsagePolicyRepository usagePolicyRepository,
    IMapper mapper
    ) : IRequestHandler<GetUsagePolicyByIdQuery, UsagePolicyResponse>
{
    public async Task<UsagePolicyResponse> Handle(GetUsagePolicyByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy UsagePolicy bằng Id: {PolicyId}", request.Id);

        // 1. Lấy entity từ repository
        var usagePolicy = await usagePolicyRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(UsagePolicy), request.Id.ToString());

        // 2. Map entity sang DTO
        var usagePolicyResponse = mapper.Map<UsagePolicyResponse>(usagePolicy);

        // 3. Trả về DTO
        return usagePolicyResponse;
    }
}
