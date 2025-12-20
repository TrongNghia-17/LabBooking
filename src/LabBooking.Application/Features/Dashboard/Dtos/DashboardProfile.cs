using LabBooking.Domain.NonEntities;

namespace LabBooking.Application.Features.Dashboard.Dtos;

public class DashboardProfile : Profile
{
    public DashboardProfile()
    {
        // Định nghĩa quy tắc:
        // Khi gặp StatItem, hãy tạo ra một StatItemDto mới.
        CreateMap<StatItem, StatItemDto>();
    }
}
