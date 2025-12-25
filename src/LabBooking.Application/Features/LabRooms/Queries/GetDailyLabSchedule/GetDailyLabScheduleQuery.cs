using LabBooking.Domain.NonEntities;

namespace LabBooking.Application.Features.LabRooms.Queries.GetDailyLabSchedule;

public record GetDailyLabScheduleQuery(DateOnly? Date) : IRequest<List<LabDailySchedule>>;