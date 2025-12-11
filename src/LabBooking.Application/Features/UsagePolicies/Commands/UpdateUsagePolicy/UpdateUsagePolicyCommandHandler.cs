using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.UsagePolicies.Commands.UpdateUsagePolicy;

public class UpdateUsagePolicyCommandHandler(
    ILogger<UpdateUsagePolicyCommandHandler> logger,
    IMapper mapper,
    IUsagePolicyRepository usagePolicyRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateUsagePolicyCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUsagePolicyCommand request, CancellationToken cancellationToken)
    {
        var updaterId = currentUserService.UserId;

        if (updaterId == null)
        {
            logger.LogWarning("Không tìm thấy thông tin người dùng (chưa đăng nhập) khi cập nhật usage policy.");
            throw new UnauthorizedAccessException("Người dùng không được xác thực.");
        }

        logger.LogInformation("Người dùng {UpdaterId} đang cập nhật usage policy {PolicyId}", updaterId.Value, request.Id);

        // 1. Lấy entity từ repository
        // (Giả định IUsagePolicyRepository có GetByIdAsync tương tự ILabRoomRepository)
        var policyToUpdate = await usagePolicyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (policyToUpdate == null)
        {
            logger.LogWarning("Usage policy with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(UsagePolicy), request.Id.ToString());
        }

        // 2. Map các thay đổi từ Command (request) vào entity (policyToUpdate)
        mapper.Map(request, policyToUpdate);

        // 3. Cập nhật ngày
        policyToUpdate.LastUpdatedDate = DateTime.UtcNow;

        // 4. Lưu thay đổi
        // (Giả định IUsagePolicyRepository có Update tương tự ILabRoomRepository)
        await usagePolicyRepository.Update(policyToUpdate, cancellationToken);

        return Unit.Value;
    }
}
