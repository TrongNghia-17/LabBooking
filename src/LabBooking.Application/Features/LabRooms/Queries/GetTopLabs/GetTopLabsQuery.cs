using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetTopLabs;

public record GetTopLabsQuery(int Year) : IRequest<IEnumerable<MonthlyTopLabDto>>;
