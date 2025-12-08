using LabBooking.Application.Features.BookingConsentRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingConsentRequest.Queries.GetRescheduleBookingConsent
{
    public record GetRescheduleBookingConsentQuery(Guid ConsentId) : IRequest<RescheduleResponse>;
}
