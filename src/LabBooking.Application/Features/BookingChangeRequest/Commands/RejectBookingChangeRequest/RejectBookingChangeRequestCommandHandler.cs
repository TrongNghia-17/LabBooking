using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.RejectBookingChangeRequest
{
    public class RejectBookingChangeRequestCommandHandler(
    IBookingChangeRequestRepository requestRepository // Inject Repository
    ) : IRequestHandler<RejectBookingChangeRequestCommand, bool>
    {
        public async Task<bool> Handle(RejectBookingChangeRequestCommand request, CancellationToken cancellationToken)
        {
            var isValidManager = await requestRepository.CheckBookingChangeRequestIsBelongToThisManager(request.BookingId, request.ManagerId);
            if (!isValidManager)
                throw new BadRequestException("Bạn không có quyền từ chối yêu cầu thay đổi này.");
            // Lưu ý: request.BookingId ở đây thực chất là ID của ChangeRequest (do FE truyền lên)
            await requestRepository.RejectChangeRequestAsync(request.BookingId, request.ManagerId);

            return true;
        }
    }
}
