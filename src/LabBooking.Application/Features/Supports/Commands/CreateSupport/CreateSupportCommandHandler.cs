using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

public class CreateSupportCommandHandler(
    ILogger<CreateSupportCommandHandler> logger,
    IMapper mapper,
    ISupportRepository supportRepository
    ) : IRequestHandler<CreateSupportCommand, SupportsResponse>
{
    public async Task<SupportsResponse> Handle(CreateSupportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new support ticket");

        var support = mapper.Map<Support>(request);

        var createdSupport = await supportRepository.Create(support);

        var response = mapper.Map<SupportsResponse>(createdSupport);
        return response;
    }
}
