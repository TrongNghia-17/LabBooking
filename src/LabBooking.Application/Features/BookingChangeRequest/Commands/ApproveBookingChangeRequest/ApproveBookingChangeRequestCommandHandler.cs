using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.ApproveBookingChangeRequest
{
    public class ApproveBookingChangeRequestCommandHandler(
    IBookingChangeRequestRepository requestRepository
    ) : IRequestHandler<ApproveBookingChangeRequestCommand, bool>
    {
        public async Task<bool> Handle(ApproveBookingChangeRequestCommand command, CancellationToken token)
        {
            var isValidManager = await requestRepository.CheckBookingChangeRequestIsBelongToThisManager(command.BookingChangeRequestId, command.ManagerId);
            if (!isValidManager)
                throw new InvalidOperationException("Bạn không có quyền từ chối yêu cầu thay đổi này.");
            await requestRepository.ApproveRequestAsync(command.BookingChangeRequestId, command.ManagerId);
            return true;
        }
    }
}
