namespace LabBooking.Application.Features.Supports.Commands.DeleteSupport;

public class DeleteSupportCommandHandler(
    ILogger<DeleteSupportCommandHandler> logger,
    ISupportRepository supportRepository
    ) : IRequestHandler<DeleteSupportCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSupportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting support ticket with Id: {Id}", request.Id);

        var supportToDelete = await supportRepository.GetByIdAsync(request.Id);

        if (supportToDelete == null)
        {
            logger.LogWarning("Support ticket with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Support), request.Id.ToString());
        }

        await supportRepository.DeleteAsync(supportToDelete);

        return Unit.Value;
    }
}
