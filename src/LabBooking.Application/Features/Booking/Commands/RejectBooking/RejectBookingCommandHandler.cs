using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Commands.RejectBooking
{
    public class RejectBookingCommandHandler(
    IBookingRepository bookingRepository // Inject Repository, KHÔNG Inject DbContext
    ) : IRequestHandler<RejectBookingCommand, bool>
    {
        public async Task<bool> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            var isValidManager = await bookingRepository.CheckBookingIsBelongToThisManager(request.BookingId, request.ManagerId);
            if (!isValidManager)
                throw new BadRequestException("Bạn không có quyền từ chối đơn này.");
            // Gọi hàm trong Repository để xử lý toàn bộ logic
            await bookingRepository.RejectBookingAsync(request.BookingId, request.ManagerId);

            return true;
        }
    }
}
