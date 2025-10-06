namespace LabBooking.Infrastructure.Repositories;

internal class IncidentRepository(LabBookingDbContext dbContext) : IIncidentRepository
{
    public async Task<Incident> Create(Incident entity)
    {
        dbContext.Incidents.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
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

}
