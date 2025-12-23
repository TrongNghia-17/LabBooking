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

        public async Task<Guid> Create(Course entity, CancellationToken cancellationToken = default)
        {
            dbContext.Courses.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task<bool> IsCourseCodeUniqueAsync(string courseCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(courseCode)) return true;

            var codeLower = courseCode.ToLower();
            return !await dbContext.Courses
                .AnyAsync(c => c.CourseCode != null && c.CourseCode.ToLower() == codeLower, cancellationToken);
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Course entity, CancellationToken cancellationToken = default)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsCourseCodeUniqueAsync(Guid id, string courseCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(courseCode)) return true;

            var codeLower = courseCode.ToLower();

            // Logic: Không được có thằng nào (Khác ID hiện tại) mà lại có cùng Code
            var isDuplicate = await dbContext.Courses
                .AnyAsync(c => c.CourseCode != null && c.CourseCode.ToLower() == codeLower && c.Id != id, cancellationToken);

            return !isDuplicate;
        }
        public async Task DeleteAsync(Course entity, CancellationToken cancellationToken = default)
        {
            dbContext.Courses.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
