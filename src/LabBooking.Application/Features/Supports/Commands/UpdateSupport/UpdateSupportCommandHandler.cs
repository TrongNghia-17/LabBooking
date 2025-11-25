namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

/// <summary>
/// Handles the business logic for the <see cref="UpdateSupportCommand"/>.
/// </summary>
public class UpdateSupportCommandHandler(
    ILogger<UpdateSupportCommandHandler> logger,
    IMapper mapper,
    ISupportRepository supportRepository
    ) : IRequestHandler<UpdateSupportCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSupportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to update support ticket {SupportId}", request.Id);

        var supportToUpdate = await supportRepository.GetByIdAsync(request.Id);

        if (supportToUpdate == null)
        {
            logger.LogWarning("Update failed: Support ticket {SupportId} was not found.", request.Id);
            throw new NotFoundException(nameof(Support), request.Id.ToString());
        }

        mapper.Map(request, supportToUpdate);

        supportToUpdate.Status = request.Status;

        if (request.Status == SupportStatus.Responded || request.Status == SupportStatus.Ignored)
        {
            supportToUpdate.RespondedAt = DateTime.UtcNow;
        }

        // Logic nghiệp vụ:
        // Nếu Admin chọn trả lời -> Bắt buộc lưu câu trả lời
        if (request.Status == SupportStatus.Responded)
        {
            supportToUpdate.Answer = request.Answer;
        }
        // Nếu Admin đánh dấu là Rác/Ignored -> Có thể xóa câu trả lời cũ hoặc để trống
        else if (request.Status == SupportStatus.Ignored)
        {
            supportToUpdate.Answer = null;
        }

        await supportRepository.Update(supportToUpdate);

        logger.LogInformation("Successfully updated support ticket {SupportId}", request.Id);

        return Unit.Value;
    }
}
