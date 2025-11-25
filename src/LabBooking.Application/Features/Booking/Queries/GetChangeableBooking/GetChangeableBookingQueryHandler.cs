using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetChangeableBooking
{
    public class GetChangeableBookingQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetChangeableBookingsQuery, List<BookingResponse>>
    {
        public async Task<List<BookingResponse>> Handle(GetChangeableBookingsQuery request, CancellationToken cancellationToken)
        {
            // Gọi Repo
            var data = await bookingRepository.GetBookingsWithChangeStatusAsync(request.UserId);

            var responseList = new List<BookingResponse>();

            foreach (var item in data)
            {
                // Dùng AutoMapper map Entity -> DTO
                var dto = mapper.Map<BookingResponse>(item.Booking);

                // Gán thêm cờ
                dto.HasPendingChangeRequest = item.HasPendingRequest;

                responseList.Add(dto);
            }

            return responseList;
        }
    }
}
