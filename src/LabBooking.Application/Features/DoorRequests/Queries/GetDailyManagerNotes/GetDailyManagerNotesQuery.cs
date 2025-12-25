using LabBooking.Domain.NonEntities;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDailyManagerNotes;

public record GetDailyManagerNotesQuery(DateOnly? Date) : IRequest<List<DailyManagerNoteDto>>;
