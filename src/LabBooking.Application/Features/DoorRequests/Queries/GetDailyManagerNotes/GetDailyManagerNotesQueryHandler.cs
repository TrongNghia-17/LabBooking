using LabBooking.Domain.NonEntities;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDailyManagerNotes;

public class GetDailyManagerNotesQueryHandler(
    IDoorRequestRepository doorRequestRepository
    ) : IRequestHandler<GetDailyManagerNotesQuery, List<DailyManagerNoteDto>>
{
    public async Task<List<DailyManagerNoteDto>> Handle(GetDailyManagerNotesQuery request, CancellationToken cancellationToken)
    {
        // Nếu không truyền ngày, mặc định lấy ngày hiện tại (Hôm nay)
        var targetDate = request.Date ?? DateOnly.FromDateTime(DateTime.Now);

        return await doorRequestRepository.GetManagerNotesByDateAsync(targetDate, cancellationToken);
    }
}