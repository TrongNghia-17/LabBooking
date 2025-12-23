using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using LabBooking.Application.Features.BookingChangeRequest.Queries.GetPendingBookingChangeRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Queries.GetBookingChangeRequest
{
    internal class GetAllChangeRequestsQueryHandler(
        IBookingChangeRequestRepository requestRepository,
        IMapper mapper
    ) : IRequestHandler<GetAllChangeRequestsQuery, List<BookingChangeRequestResponse>>
    {
        public async Task<List<BookingChangeRequestResponse>> Handle(GetAllChangeRequestsQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi Repo lấy data
            var requests = await requestRepository.GetAllRequestsAsync(request.UserId);

            // 2. Map sang DTO (BookingChangeRequestResponse đã cấu hình ở bước trước)
            var bookingChangeRequestResponses = mapper.Map<List<BookingChangeRequestResponse>>(requests);
            return bookingChangeRequestResponses;
        }
    }
}
