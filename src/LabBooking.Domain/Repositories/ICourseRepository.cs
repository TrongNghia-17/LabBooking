namespace LabBooking.Domain.Repositories
{
    public interface ICourseRepository
    {
        Task<(IEnumerable<Course>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection);
        Task DeleteAsync(Course entity, CancellationToken cancellationToken = default);
        Task<Guid> Create(Course entity, CancellationToken cancellationToken = default);
        Task<bool> IsCourseCodeUniqueAsync(string courseCode, CancellationToken cancellationToken = default);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Course entity, CancellationToken cancellationToken = default);

        // Method mới: Check trùng mã nhưng loại trừ ID hiện tại
        Task<bool> IsCourseCodeUniqueAsync(Guid id, string courseCode, CancellationToken cancellationToken = default);
    }
}
