using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Courses.Commands.CreateCourse;
using LabBooking.Application.Features.Courses.Commands.DeleteCourse;
using LabBooking.Application.Features.Courses.Commands.UpdateCourse;
using LabBooking.Application.Features.Courses.Dtos;
using LabBooking.Application.Features.Courses.Queries.GetAllCourses;
using LabBooking.Application.Features.Courses.Queries.GetCourseById;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(
    IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Tạo mới một học phần (Course)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được tạo môn học
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
        {
            var id = await mediator.Send(command);

            // Khi bạn làm xong phần GetById, hãy sửa null thành CreatedAtAction(nameof(GetById), new { id }, id)
            return StatusCode(StatusCodes.Status201Created, new { Id = id });
        }

        /// <summary>
        /// Cập nhật thông tin học phần
        /// </summary>
        /// <param name="id">ID của học phần cần sửa</param>
        /// <param name="command">Thông tin cập nhật</param>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCourseCommand command)
        {
            if (id != command.Id && command.Id != Guid.Empty)
            {
                return BadRequest("ID trong URL không khớp với ID trong body.");
            }

            command.Id = id; // Đảm bảo ID chính xác
            await mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Xóa một học phần
        /// </summary>
        /// <param name="id">ID của học phần cần xóa</param>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được xóa
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await mediator.Send(new DeleteCourseCommand(id));
                return NoContent(); // Trả về 204 khi xóa thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin, Manager, User")]
        [ProducesResponseType(typeof(PagedResult<CourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<CourseResponse>>> GetAll([FromQuery] GetAllCoursesQuery query)
        {
            var courses = await mediator.Send(query);
            return Ok(courses);
        }

        /// <summary>
        /// Lấy thông tin chi tiết học phần theo ID
        /// </summary>
        /// <param name="id">ID của học phần</param>
        /// <returns>Thông tin học phần</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin, Manager, User")] // Ai cũng xem được (tùy business của bạn)
        [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseResponse>> GetById([FromRoute] Guid id)
        {
            try
            {
                var response = await mediator.Send(new GetCourseByIdQuery(id));
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy học phần yêu cầu.");
            }
        }
    }
}
