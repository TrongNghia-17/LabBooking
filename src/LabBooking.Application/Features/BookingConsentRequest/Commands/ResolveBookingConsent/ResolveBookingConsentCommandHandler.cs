using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingConsentRequest.Commands.ResolveBookingConsent
{
    public class ResolveBookingConsentCommandHandler(
        IBookingConsentRequestRepository consentRepository, // Inject Repository thay vì DbContext
        ILogger<ResolveBookingConsentCommandHandler> logger
        ) : IRequestHandler<ResolveBookingConsentCommand, bool>
    {
        public async Task<bool> Handle(ResolveBookingConsentCommand request, CancellationToken cancellationToken)
        {
            // Switch case để điều hướng hành động
            if (request.Action == "Cancel")
            {
                // Gọi hàm trong Repository để xử lý toàn bộ logic
                await consentRepository.ConfirmConsentCancelAsync(request.ConsentId, cancellationToken);
            }
            else if (request.Action == "Reschedule")
            {
                // Todo: Sau này bạn sẽ viết thêm hàm repo.ConfirmRescheduleAsync(...)
                throw new NotImplementedException("Logic Reschedule sẽ được cập nhật sau.");
            }
            else
            {
                throw new ArgumentException($"Hành động '{request.Action}' không hợp lệ.");
            }

            return true;
        }
    }
}
