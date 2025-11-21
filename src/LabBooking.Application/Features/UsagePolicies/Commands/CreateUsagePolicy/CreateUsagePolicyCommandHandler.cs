using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.UsagePolicies.Commands.CreateUsagePolicy;

/// <summary>
/// Handles the <see cref="CreateUsagePolicyCommand"/>.
/// </summary>
public class CreateUsagePolicyCommandHandler(
    ILogger<CreateUsagePolicyCommandHandler> logger,
    IMapper mapper,
    IUsagePolicyRepository usagePolicyRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateUsagePolicyCommand, Guid>
{
    public async Task<Guid> Handle(CreateUsagePolicyCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUserService.UserId;

        if (creatorId == null)
        {
            logger.LogWarning("Không tìm thấy thông tin người dùng (chưa đăng nhập) khi tạo usage policy.");
            throw new UnauthorizedAccessException("Người dùng không được xác thực.");
        }

        logger.LogInformation("Người dùng {CreatorId} đang tạo usage policy mới: {Title}", creatorId.Value, request.Title);

        // Map command (DTO) sang entity
        var usagePolicy = mapper.Map<UsagePolicy>(request);

        // Gán các giá trị bổ sung
        usagePolicy.CreatedById = creatorId.Value;
        // Các giá trị mặc định như Id, CreatedDate, IsActive đã được tự gán trong entity UsagePolicy.cs

        // Lưu vào database
        var usagePolicyId = await usagePolicyRepository.Create(usagePolicy, cancellationToken);

        return usagePolicyId;
    }
}
