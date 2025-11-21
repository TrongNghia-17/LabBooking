using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class CourseRepository(LabBookingDbContext dbContext) : ICourseRepository
    {
        public async Task<(IEnumerable<Course>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
        {
            var searchPhraseLower = searchPhrase?.ToLower();

            var baseQuery = dbContext
                .Courses
                .Where(r => searchPhraseLower == null ||
                            r.CourseName.ToLower().Contains(searchPhraseLower) ||
                            r.CourseCode.ToLower().Contains(searchPhraseLower));

            var totalCount = await baseQuery.CountAsync();

            if (sortBy != null)
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Course, object>>>
            {
                { nameof(Course.CourseName), r => r.CourseName },
                { nameof(Course.CourseCode), r => r.CourseCode }
            };

                if (columnsSelector.ContainsKey(sortBy))
                {
                    var selectedColumn = columnsSelector[sortBy];

                    baseQuery = sortDirection == SortDirection.Ascending
                        ? baseQuery.OrderBy(selectedColumn)
                        : baseQuery.OrderByDescending(selectedColumn);
                }
            }

            var courses = await baseQuery
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (courses, totalCount);
        }
    }
}
