using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
