using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAvailableLabsByDate;

public class GetAvailableLabsByDateQuery : IRequest<IEnumerable<LabRoomAvailabilityDto>>
{
    public DateOnly Date { get; set; }

    public GetAvailableLabsByDateQuery(DateOnly date)
    {
        Date = date;
    }
}
