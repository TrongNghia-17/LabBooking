using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingPriorityDetail.Dtos
{
    public record BookingPriorityDetailResponse
    (
        Guid Id,
        string Justification,
        string? EvidenceFilePath,
        Guid BookingId,
        Guid? ApprovedById,
        DateTime? ApprovedAt,
        string? ManagerNote
    );
}
