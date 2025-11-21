using LabBooking.Application.Features.Slots.Dtos;
using LabBooking.Application.Features.Supports.Dtos;
using LabBooking.Application.Features.Supports.Queries.GetAllSupports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Slots.Queries.GetAllSlots
{
    public class GetAllSlotsQueryHandle(
    ILogger<GetAllSlotsQueryHandle> logger,
    ISlotRepository slotRepository,
    IMapper mapper) : IRequestHandler<GetAllSlotsQuery, IEnumerable<SlotResponse>>
    {
        public async Task<IEnumerable<SlotResponse>> Handle(
        GetAllSlotsQuery request,
        CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing GetAllSlotsQuery");

            var (slots, totalCount) = await slotRepository.GetAllSlotAsync(cancellationToken);

            var slotResponses = mapper.Map<IEnumerable<SlotResponse>>(slots);

            return slotResponses;
        }
    }
}
