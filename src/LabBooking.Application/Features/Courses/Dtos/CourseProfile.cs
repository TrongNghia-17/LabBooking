using LabBooking.Application.Features.Courses.Commands.CreateCourse;
using LabBooking.Application.Features.Courses.Commands.UpdateCourse;

namespace LabBooking.Application.Features.Courses.Dtos
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseResponse>().ReverseMap();
            CreateMap<CreateCourseCommand, Course>();
            CreateMap<UpdateCourseCommand, Course>();
        }
    }
}
