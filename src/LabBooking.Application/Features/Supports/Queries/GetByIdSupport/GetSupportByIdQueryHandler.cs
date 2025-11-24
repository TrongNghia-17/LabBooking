using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetByIdSupport;

/// <summary>
/// Handles the execution of the <see cref="GetSupportByIdQuery"/>.
/// </summary>
/// <remarks>
/// This handler retrieves a specific support ticket from the repository by its ID,
/// maps it to a <see cref="SupportsResponse"/> DTO, and returns it.
/// </remarks>
public class GetSupportByIdQueryHandler(
    ILogger<GetSupportByIdQueryHandler> logger,
    ISupportRepository supportRepository,
    IMapper mapper) : IRequestHandler<GetSupportByIdQuery, SupportsResponse>
{
    /// <summary>
    /// Handles the <see cref="GetSupportByIdQuery"/>.
    /// </summary>
    /// <param name="request">The query request containing the ID of the support ticket.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="SupportsResponse"/> DTO representing the found support ticket.</returns>
    /// <exception cref="NotFoundException">Thrown if no support ticket is found with the specified ID.</exception>
    public async Task<SupportsResponse> Handle(GetSupportByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetSupportByIdQuery for SupportId: {SupportId}", request.Id);

        var support = await supportRepository.GetByIdAsync(request.Id);
        if (support == null)
        {
            logger.LogWarning("Support ticket with Id: {SupportId} was not found.", request.Id);
            throw new NotFoundException(nameof(Support), request.Id.ToString());
        }

        var supportResponse = mapper.Map<SupportsResponse>(support);

        logger.LogInformation("Successfully retrieved support ticket with Id: {SupportId}", request.Id);

        return supportResponse;
    }
}
