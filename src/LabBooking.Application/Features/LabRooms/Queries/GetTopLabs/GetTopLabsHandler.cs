using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetTopLabs;

public class GetTopLabsHandler(ILabRoomRepository repo)
    : IRequestHandler<GetTopLabsQuery, IEnumerable<MonthlyTopLabDto>>
{
    public async Task<IEnumerable<MonthlyTopLabDto>> Handle(GetTopLabsQuery request, CancellationToken cancellationToken)
    {
        // Nếu không truyền năm thì lấy năm nay
        int targetYear = request.Year > 0 ? request.Year : DateTime.UtcNow.Year;

        return await repo.GetTopLabPerMonthAsync(targetYear, cancellationToken);
    }
}
