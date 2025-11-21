namespace LabBooking.Application.Features.UsagePolicies.Commands.DeleteUsagePolicy;

public class DeleteUsagePolicyCommandHandler(
    ILogger<DeleteUsagePolicyCommandHandler> logger,
    IUsagePolicyRepository usagePolicyRepository // Sử dụng IUsagePolicyRepository
    ) : IRequestHandler<DeleteUsagePolicyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUsagePolicyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang xóa usage policy với Id: {Id}", request.Id);

        // 1. Tìm entity
        var policyToDelete = await usagePolicyRepository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra NotFound (giống DeleteLabRoomCommandHandler)
        if (policyToDelete == null)
        {
            logger.LogWarning("Usage policy with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(UsagePolicy), request.Id.ToString());
        }

        // 3. Xóa
        await usagePolicyRepository.DeleteAsync(policyToDelete, cancellationToken);

        return Unit.Value;
    }
}