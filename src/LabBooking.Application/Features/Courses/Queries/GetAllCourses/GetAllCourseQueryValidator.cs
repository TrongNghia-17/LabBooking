using LabBooking.Application.Features.Courses.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
    {
        private readonly int[] allowPageSizes = [5, 10, 15, 30];

        // Cho phép sort theo Tên hoặc Mã
        private readonly string[] allowedSortByColumnNames =
            [nameof(CourseResponse.CourseName), nameof(CourseResponse.CourseCode)];

        public GetAllCoursesQueryValidator()
        {
            RuleFor(r => r.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(r => r.PageSize)
                .Must(value => allowPageSizes.Contains(value))
                .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

            RuleFor(r => r.SortBy)
                .Must(value => allowedSortByColumnNames.Contains(value))
                .When(q => q.SortBy != null)
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }
    }
}
