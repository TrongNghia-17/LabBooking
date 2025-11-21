using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class BookingPriorityDetail
    {
        public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
        public Guid BookingId { get; set; }

        // --- Thông tin User khai báo ---
        // Lý do tại sao cần ưu tiên? (VD: Sự kiện kỷ niệm 20 năm thành lập trường)
        public string Justification { get; set; } = string.Empty;

        // Đường dẫn tới file minh chứng (PDF quyết định của Hiệu trưởng, v.v.)
        public string? EvidenceFilePath { get; set; }
        public string? ManagerNote { get; set; }
    }
}
