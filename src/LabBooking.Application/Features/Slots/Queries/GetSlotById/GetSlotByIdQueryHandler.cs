using LabBooking.Application.Features.Slots.Dtos;

namespace LabBooking.Application.Features.Slots.Queries.GetSlotById;

public class GetSlotByIdQueryHandler(
    ISlotRepository slotRepository,
    IMapper mapper
    ) : IRequestHandler<GetSlotByIdQuery, SlotResponse>
{
    public async Task<SlotResponse> Handle(GetSlotByIdQuery request, CancellationToken cancellationToken)
    {
        var slot = await slotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (slot == null)
        {
            // Ném lỗi này để Controller hoặc Middleware bắt và trả về 404
            throw new KeyNotFoundException($"Không tìm thấy Slot với ID: {request.Id}");
        }

        return mapper.Map<SlotResponse>(slot);
    }
}
