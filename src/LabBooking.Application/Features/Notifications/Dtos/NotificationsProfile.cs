namespace LabBooking.Application.Features.Notifications.Dtos;

public class NotificationsProfile : Profile
{
    public NotificationsProfile()
    {
        CreateMap<Notification, NotificationsResponse>();
    }
}
