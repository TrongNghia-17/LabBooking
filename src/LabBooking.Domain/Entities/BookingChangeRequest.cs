using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class BookingChangeRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Link về Booking gốc
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }

        public Guid RequestedById { get; set; }

        // --- NHÓM 1: CÁC TRƯỜNG CƠ BẢN (Lưu cột riêng) ---
        public string? NewTitle { get; set; }
        public string? NewDescription { get; set; }
        public int? NewNumberOfParticipants { get; set; }

        // Nếu đổi môn học
        public Guid? NewCourseId { get; set; }

        // --- NHÓM 2: CÁC OBJECT PHỨC TẠP (Lưu JSON String) ---

        // Chứa: { "ProjectName": "...", "Description": "...", "ProjectType": 1 }
        public string? NewProjectJson { get; set; }

        // Chứa: { "Justification": "...", "EvidenceFilePath": "..." }
        public string? NewPriorityDetailJson { get; set; }

        // Chứa List: [{ "Name": "...", "Quantity": 2, "Description": "..." }]
        public string? NewExternalEquipmentsJson { get; set; }

        public string? NewOutSideGuestsJson { get; set; }

        // --- NHÓM 3: DANH SÁCH SLOT MONG MUỐN (Bảng con) ---
        // Lưu toàn bộ slot user muốn giữ (cũ + mới)
        public ICollection<BookingChangeRequestSlot> NewSlots { get; set; }

        // --- QUẢN LÝ TRẠNG THÁI ---
        public BookingChangeRequestStatus Status { get; set; } = BookingChangeRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public Guid? ProcessedById { get; set; }
        public string? ManagerReason { get; set; }

        // Để phân biệt đâu là đơn user xin, đâu là đơn hệ thống đền bù
        public RequestType RequestType { get; set; } = RequestType.UserRequest;

        // Lưu vết các slot bị mất (để FE hiển thị: "Bạn bị mất slot ngày X do sự kiện Y")
        public string? OriginalOverriddenSlotsJson { get; set; }
    }

    // Bảng con lưu slot
    public class BookingChangeRequestSlot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BookingChangeRequestId { get; set; }
        public Guid SlotId { get; set; }
        public DateOnly Date { get; set; }
    }

    public enum BookingChangeRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public enum RequestType
    {
        UserRequest,        // User tự tạo (Xin đổi lịch)
        SystemOverride      // [MỚI] Hệ thống tạo do bị Priority đè
    }
}
