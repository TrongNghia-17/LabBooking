using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.ApproveBooking.Dtos
{
    public record ApproveBookingResponse(
    bool IsSuccess,
    string Message,
    string Status // "Approved" hoặc "WaitingForConsent"
);
}
