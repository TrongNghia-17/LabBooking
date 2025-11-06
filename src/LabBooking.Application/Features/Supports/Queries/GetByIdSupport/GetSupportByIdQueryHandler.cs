using LabBooking.Application.Features.Supports.Dtos;

namespace LabBooking.Application.Features.Supports.Queries.GetByIdSupport;

public class GetSupportByIdQueryHandler(
    ILogger<GetSupportByIdQueryHandler> logger,
    ISupportRepository supportRepository,
    IMapper mapper) : IRequestHandler<GetSupportByIdQuery, SupportsResponse>
{
    public async Task<SupportsResponse> Handle(GetSupportByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Support by Id: {SupportId}", request.Id);

        var support = await supportRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Support), request.Id.ToString());

        var supportResponse = mapper.Map<SupportsResponse>(support);

        return supportResponse;
    }
}
