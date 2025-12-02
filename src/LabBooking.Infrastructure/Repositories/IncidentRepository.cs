using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class IncidentRepository(LabBookingDbContext dbContext) : IIncidentRepository
{
    public async Task<Guid> CreateAsync(Incident incident, CancellationToken token)
    {
        await dbContext.Incidents.AddAsync(incident, token);
        await dbContext.SaveChangesAsync(token);

        return incident.Id;
    }

    public async Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .Incidents
            .Where(r => searchPhraseLower == null || r.Description.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Incident, object>>>
            {
                { nameof(Incident.Description), r => r.Description },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var labs = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (labs, totalCount);
    }

    public async Task<bool> IsSpamAsync(Guid userId, Guid labRoomId, IncidentType type, CancellationToken token)
    {
        var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);

        return await dbContext.Incidents
            .AnyAsync(x => x.ReportedById == userId
                        && x.LabRoomId == labRoomId
                        && x.Type == type
                        && x.CreatedAt > oneMinuteAgo, token);
    }
}
