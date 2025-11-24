using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.BookingPriorityDetail.Dtos;
using LabBooking.Application.Features.BookingSlots.Dtos;
using LabBooking.Application.Features.Course.Dtos;
using LabBooking.Application.Features.ExternalEquipment.Dtos;
using LabBooking.Application.Features.LabRooms.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Dtos
{
    public class BookingChangeRequestResponse
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        //public BookingResponse? Booking { get; set; }
        public Guid RequestedById { get; set; }

        // --- NHÓM 1: CÁC TRƯỜNG CƠ BẢN (Y chang Entity) ---
        public string? NewTitle { get; set; }
        public string? NewDescription { get; set; }
        public int? NewNumberOfParticipants { get; set; }
        public Guid? NewCourseId { get; set; }

        // --- NHÓM 2: CÁC OBJECT PHỨC TẠP (Đã bung từ JSON ra Object) ---

        // Thay vì string NewProjectJson -> Trả về Object
        public ProjectResponse? NewProject { get; set; }

        // Thay vì string NewPriorityDetailJson -> Trả về Object
        public BookingPriorityDetailResponse? NewPriorityDetail { get; set; }

        // Thay vì string NewExternalEquipmentsJson -> Trả về List Object
        public List<ExternalEquipmentResponse>? NewExternalEquipments { get; set; }

        // --- NHÓM 3: SLOTS ---
        public List<BookingChangeRequestSlot> NewSlots { get; set; }

        // --- NHÓM 4: TRẠNG THÁI ---
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // --- [BỔ SUNG] THÔNG TIN TỪ BOOKING GỐC ---
        // (Cần thiết để FE hiển thị tên phòng, loại booking...)
        public string OriginalType { get; set; }
        public string RoomName { get; set; }
        public Guid LabRoomId { get; set; }
        public RequestType RequestType { get; set; }
        public string? OriginalOverriddenSlotsJson { get; set; }
    }
}
