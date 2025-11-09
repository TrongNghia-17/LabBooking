using LabBooking.Application.Features.Supports.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

/// <summary>
/// Handles the logic for the <see cref="CreateSupportCommand"/>.
/// </summary>
public class CreateSupportCommandHandler(
    ILogger<CreateSupportCommandHandler> logger,
    IMapper mapper,
    ISupportRepository supportRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateSupportCommand, Guid>
{
    public async Task<Guid> Handle(CreateSupportCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the user ID from the authenticated context.
        var creatorId = currentUserService.UserId;

        // Ensure the user is authenticated.
        if (creatorId == null)
        {
            logger.LogWarning("Authenticated user ID not found in the current context. Creation denied.");
            throw new UnauthorizedAccessException("User must be authenticated to create a support ticket.");
        }

        logger.LogInformation("User {CreatorId} initiated creation of a new support ticket", creatorId.Value);

        var support = mapper.Map<Support>(request);
        support.CreatedById = creatorId.Value;

        var supportId = await supportRepository.Create(support);
        logger.LogInformation("Successfully created support ticket {SupportId} for user {CreatorId}", supportId, creatorId.Value);

        return supportId;
    }
}
