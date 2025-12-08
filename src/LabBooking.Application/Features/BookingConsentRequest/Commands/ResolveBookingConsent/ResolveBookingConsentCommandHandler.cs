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
            if (string.IsNullOrWhiteSpace(request.Action))
                throw new ArgumentException("Hành động không được để trống.");

            switch (request.Action)
            {
                case "Cancel":
                    await consentRepository.ConfirmConsentCancelAsync(request.ConsentId, cancellationToken);
                    break;

                case "Reschedule":
                    if (request.NewSlots == null || !request.NewSlots.Any())
                    {
                        // Validate danh sách không được rỗng
                        throw new ArgumentException("Danh sách slot mới không được để trống khi đổi lịch.");
                    }

                    // Gọi hàm tạo request bên Repository
                    await consentRepository.CreateRescheduleRequestAsync(request.ConsentId, request.NewSlots, cancellationToken);
                    break;

                default:
                    throw new ArgumentException($"Hành động '{request.Action}' không hợp lệ.");
            }

            return true;
        }
    }
}
