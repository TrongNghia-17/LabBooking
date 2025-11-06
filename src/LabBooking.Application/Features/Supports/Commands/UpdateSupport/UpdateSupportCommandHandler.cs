namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

public class UpdateSupportCommandHandler(
    ILogger<UpdateSupportCommandHandler> logger,
    IMapper mapper,
    ISupportRepository supportRepository
    ) : IRequestHandler<UpdateSupportCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSupportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating support ticket with Id: {Id}", request.Id);

        var supportToUpdate = await supportRepository.GetByIdAsync(request.Id);

        if (supportToUpdate == null)
        {
            logger.LogWarning("Support ticket with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Support), request.Id.ToString());
        }

        mapper.Map(request, supportToUpdate);

        await supportRepository.Update(supportToUpdate);

        return Unit.Value;
    }
}
