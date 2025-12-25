using LabBooking.Application.Features.ExternalEquipment.Dtos;

namespace LabBooking.Application.Features.Booking.Dtos
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // =========================================================
            // 1. MAP BOOKING CHA (Entity -> Response)
            // =========================================================
            CreateMap<Domain.Entities.Booking, BookingResponse>()
                // --- Map Enum sang String/Int ---
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))

                // --- Map Object con (Nested Objects) ---
                // AutoMapper sẽ tự động tìm các Map con (được khai báo bên dưới) để map dữ liệu
                .ForMember(dest => dest.labRoomResponse, opt => opt.MapFrom(src => src.LabRoom))
                .ForMember(dest => dest.projectResponse, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.courseResponse, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.PriorityDetail, opt => opt.MapFrom(src => src.BookingPriorityDetail))

                // --- Map Lists (Collections) ---
                .ForMember(dest => dest.Slots, opt => opt.MapFrom(src => src.Slots))
                .ForMember(dest => dest.ExternalEquipments, opt => opt.MapFrom(src => src.ExternalEquipments));



            // =========================================================
            // 2. KHAI BÁO CÁC MAP CON (BẮT BUỘC PHẢI CÓ)
            // =========================================================

            // Map Project -> ProjectResponse
            CreateMap<Project, ProjectResponse>();

            //// Map Course -> CourseResponse (Giả sử bạn đã có class CourseResponse)
            //CreateMap<Domain.Entities.Course, CourseResponse>();

            //// Map BookingPriorityDetail -> BookingPriorityDetailResponse
            //CreateMap<Domain.Entities.BookingPriorityDetail, BookingPriorityDetailResponse>();

            //// Map BookingSlot -> BookingSlotResponse
            //CreateMap<BookingSlot, BookingSlotResponse>()
            //     .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()));

            //// Map ExternalEquipment -> ExternalEquipmentResponse
            CreateMap<Domain.Entities.ExternalEquipment, ExternalEquipmentResponse>();
            CreateMap<Domain.Entities.OutSideGuest, OutSideGuestResponse>();
            CreateMap<Domain.Entities.Booking, BookingHistoryResponse>();
            CreateMap<LabRoom, BookingHistoryLabDto>();
            CreateMap<BookingSlot, BookingHistorySlotDto>();

            // -----Nghia------
            CreateMap<Domain.Entities.Booking, BookingLookupDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))

               // Map Code: Ưu tiên lấy QrCodeString, nếu null thì lấy Id
               .ForMember(dest => dest.BookingCode, opt => opt.MapFrom(src => src.QrCodeString ?? src.Id.ToString()))

               // Map LabName: Check null an toàn
               .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabRoom != null ? src.LabRoom.LabName : "Phòng không xác định"));
        }
    }
}
