using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public enum ConsentStatus
    {
        Pending,        // Đang chờ User quyết định
        AcceptedCancel, // User đồng ý hủy (chấp nhận mất)
        Rescheduled     // User đã chọn lịch mới
    }

    public class BookingConsentRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Đơn Booking của "Nạn nhân" (Người bị đè)
        public Guid BookingId { get; set; }

        // ID của Nạn nhân (để dễ query thông báo)
        public Guid CreatedById { get; set; }

        // Đơn Booking Priority (Thủ phạm gây ra vụ đè)
        public Guid PriorityBookingId { get; set; }

        // Danh sách ID các slot bị mất (Lưu dạng JSON string: ["guid1", "guid2"])
        public string OverriddenSlotIdsJson { get; set; }

        public ConsentStatus Status { get; set; } = ConsentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
