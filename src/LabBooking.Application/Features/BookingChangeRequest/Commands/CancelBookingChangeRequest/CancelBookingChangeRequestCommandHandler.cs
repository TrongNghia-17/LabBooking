using LabBooking.Application.Features.BookingChangeRequest.Commands.RejectBookingChangeRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.CancelBookingChangeRequest
{
    public class CancelBookingChangeRequestCommandHandler(
        IBookingChangeRequestRepository requestRepository // Inject Repository
    ) : IRequestHandler<CancelBookingChangeRequestCommand, bool>
    {
        public async Task<bool> Handle(CancelBookingChangeRequestCommand request, CancellationToken cancellationToken)
        {
            // Lưu ý: request.BookingId ở đây thực chất là ID của ChangeRequest (do FE truyền lên)
            await requestRepository.CancelChangeRequestAsync(request.Id, request.UserId);

            return true;
        }
    }
}
