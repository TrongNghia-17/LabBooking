namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentProfile : Profile
{
    public IncidentProfile()
    {
        CreateMap<Incident, IncidentResponse>()
            // 1. Map Tên Phòng (Phòng thủ nếu LabRoom bị null)
            .ForMember(dest => dest.LabRoomName, opt => opt.MapFrom(src =>
                src.LabRoom != null ? src.LabRoom.LabName : "Không xác định"))

            // 2. Map Trạng thái (Boolean -> Tiếng Việt)
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.IsResolved ? "Đã xử lý" : "Chưa xử lý"))

            // 3. Map Thiết bị (Chỉ lấy tên nếu có thiết bị, ngược lại trả về null)
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src =>
                src.Equipment != null ? src.Equipment.EquipmentName : null))

            // 4. Map Người báo cáo (Xử lý null để tránh lỗi)
            .ForMember(dest => dest.ReportedByName, opt => opt.MapFrom(src =>
                src.ReportedBy != null ? src.ReportedBy.FullName : "Ẩn danh"))

            // Map Số điện thoại (Nếu không có SĐT trong User thì trả về null)
            .ForMember(dest => dest.ReportedByPhone, opt => opt.MapFrom(src =>
                src.ReportedBy != null ? src.ReportedBy.PhoneNumber : null))

            // 5. Map Enum sang String
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ImportanceLevel, opt => opt.MapFrom(src => src.ImportanceLevel.ToString()));
    }
}