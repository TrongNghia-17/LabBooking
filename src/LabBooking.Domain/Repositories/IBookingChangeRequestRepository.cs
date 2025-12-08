using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IBookingChangeRequestRepository
    {
        Task<BookingChangeRequest> CreateAsync(BookingChangeRequest request);
        Task<bool> IsBookingOwnerAndApprovedAsync(Guid bookingId, Guid userId);
        Task<List<BookingChangeRequest>> GetPendingRequestsAsync(Guid? userId);
        Task RejectChangeRequestAsync(Guid requestId, Guid managerId, string reason);
        Task<bool> CheckBookingChangeRequestIsBelongToThisManager(Guid bookingChangeId, Guid managerId);

        // Hàm này sẽ chứa toàn bộ logic "Apply Changes" phức tạp
        Task ApproveRequestAsync(Guid requestId, Guid managerId);

        // Hàm hỗ trợ lấy Request kèm đầy đủ thông tin để check trùng
        Task<BookingChangeRequest?> GetRequestWithDetailsAsync(Guid requestId);
    }
}
