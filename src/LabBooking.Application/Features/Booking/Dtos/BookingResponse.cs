using LabBooking.Application.Features.BookingPriorityDetail.Dtos;
using LabBooking.Application.Features.BookingSlots.Dtos;
using LabBooking.Application.Features.Courses.Dtos;
using LabBooking.Application.Features.ExternalEquipment.Dtos;
using LabBooking.Application.Features.LabRooms.Dtos;
using Medo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Dtos
{
    public record BookingResponse
    {
        public Guid Id { get; set; }
        public Guid LabRoomId { get; set; }
        public LabRoomResponse? labRoomResponse { get; set; }
        public Guid CreatedById { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Trả về String để FE dễ hiển thị (Pending, Approved...)
        public string Status { get; set; } = string.Empty;

        // Trả về String (Teaching, Project, UniversityEvent)
        public string Type { get; set; } = string.Empty;

        // Trả về Int (0, 1, 2) để FE so sánh quyền lực nếu cần
        public int Priority { get; set; }

        public bool IsPublic { get; set; }
        public bool IsMajorOnly { get; set; }
        public int? NumberOfParticipants { get; set; }

        // --- THÔNG TIN RIÊNG (Sẽ null tùy theo Type) ---

        // Nếu là Teaching
        public CourseResponse? courseResponse { get; set; }

        // Nếu là Project
        public ProjectResponse? projectResponse { get; set; }

        // Nếu là Priority
        public BookingPriorityDetailResponse? PriorityDetail { get; set; }

        // Danh sách các slot đã đặt thành công
        public ICollection<BookingSlotResponse>? Slots { get; set; }

        public ICollection<ExternalEquipmentResponse>? ExternalEquipments { get; set; }
        public string? PendingSlotsJson { get; set; }
    }

    public record ProjectResponse
    (
        Guid Id,
        string ProjectName,
        string? Description,
        Guid OwnerId,
        ProjectType ProjectType
    );
}
