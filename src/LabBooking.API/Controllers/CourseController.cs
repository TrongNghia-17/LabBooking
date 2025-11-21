using LabBooking.Application.Features.Course.Dtos;
using LabBooking.Application.Features.Course.Queries.GetAllCourses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(
    IMediator mediator) : ControllerBase
    {
        [HttpGet]
        //[Authorize(Roles = "Admin")]
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
    }
}
