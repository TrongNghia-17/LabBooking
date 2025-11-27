using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Queries.GetPendingBookingChangeRequest
{
    internal class GetPendingChangeRequestQueryHandler(
        IBookingChangeRequestRepository requestRepository,
        IMapper mapper
    ) : IRequestHandler<GetPendingChangeRequestQuery, List<BookingChangeRequestResponse>>
    {
        public async Task<List<BookingChangeRequestResponse>> Handle(GetPendingChangeRequestQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi Repo lấy data
            var requests = await requestRepository.GetPendingRequestsAsync(request.UserId);

            // 2. Map sang DTO (BookingChangeRequestResponse đã cấu hình ở bước trước)
            var bookingChangeRequestResponses = mapper.Map<List<BookingChangeRequestResponse>>(requests);
            return bookingChangeRequestResponses;
        }
    }
}
