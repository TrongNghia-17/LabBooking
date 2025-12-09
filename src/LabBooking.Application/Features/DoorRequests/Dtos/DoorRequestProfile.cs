using LabBooking.Application.Features.DoorRequests.Commands.Create;

namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestProfile : Profile
{
    public DoorRequestProfile()
    {
        CreateMap<DoorOpeningRequest, DoorRequestHistoryDto>()
            // Map tên phòng
            .ForMember(dest => dest.LabRoomName, opt => opt.MapFrom(src =>
                src.LabRoom != null ? src.LabRoom.LabName : "Không xác định"))

            // Map thông tin Người gửi (Sinh viên)
            .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src =>
                src.RequestedBy != null ? src.RequestedBy.FullName : "Ẩn danh"))
            .ForMember(dest => dest.RequestedByCode, opt => opt.MapFrom(src =>
                src.RequestedBy != null ? src.RequestedBy.UserName : ""))

            // Map thông tin Người xử lý (Bảo vệ)
            .ForMember(dest => dest.HandledByName, opt => opt.MapFrom(src =>
                src.HandledBy != null ? src.HandledBy.FullName : ""))
            .ForMember(dest => dest.HandledByPhone, opt => opt.MapFrom(src =>
                src.HandledBy != null ? src.HandledBy.PhoneNumber : ""))

            // Map Enum sang String
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<CreateDoorRequestCommand, DoorOpeningRequest>()
            // 1. Map các trường trùng tên tự động (LabRoomId, Type)

            // 2. Thiết lập các giá trị mặc định hệ thống
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.RequestTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DoorRequestStatus.Pending))

            // 3. Bỏ qua các trường sẽ gán thủ công trong Handler (User) hoặc Null
            .ForMember(dest => dest.RequestedById, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LabRoom, opt => opt.Ignore())
            .ForMember(dest => dest.HandledById, opt => opt.Ignore())
            .ForMember(dest => dest.HandledBy, opt => opt.Ignore())
            .ForMember(dest => dest.BookingId, opt => opt.Ignore())
            .ForMember(dest => dest.Booking, opt => opt.Ignore());
    }
}
