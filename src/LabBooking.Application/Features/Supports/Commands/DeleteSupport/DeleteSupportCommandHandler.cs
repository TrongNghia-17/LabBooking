namespace LabBooking.Application.Features.Supports.Commands.DeleteSupport;

/// <summary>
/// Handles the deletion of a support ticket.
/// </summary>
/// <remarks>
/// This handler retrieves a support ticket by its ID, checks for its existence,
/// and then removes it from the repository.
/// </remarks>
public class DeleteSupportCommandHandler(
    ILogger<DeleteSupportCommandHandler> logger,
    ISupportRepository supportRepository
    ) : IRequestHandler<DeleteSupportCommand, Unit>
{
    /// <summary>
    /// Handles the <see cref="DeleteSupportCommand"/>.
    /// </summary>
    /// <param name="request">The command request containing the ID of the support ticket to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Unit"/> value signifying the operation is complete.</returns>
    /// <exception cref="NotFoundException">Thrown if the support ticket with the specified ID is not found.</exception>
    public async Task<Unit> Handle(DeleteSupportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing request to delete support ticket with Id: {SupportTicketId}", request.Id);

        var supportToDelete = await supportRepository.GetByIdAsync(request.Id);

        if (supportToDelete == null)
        {
            logger.LogWarning("Support ticket with Id: {SupportTicketId} was not found.", request.Id);
            throw new NotFoundException(nameof(Support), request.Id.ToString());
        }

        await supportRepository.DeleteAsync(supportToDelete);
        logger.LogInformation("Successfully deleted support ticket with Id: {SupportTicketId}", request.Id);

        return Unit.Value;
    }
}
