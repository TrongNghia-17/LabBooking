using LabBooking.Application.Features.EquipmentCategories.Commands.Create;
using LabBooking.Application.Features.EquipmentCategories.Commands.Update;
using LabBooking.Application.Features.EquipmentCategories.Dtos;
using LabBooking.Application.Features.EquipmentCategories.Queries.GetAll;
using LabBooking.Application.Features.EquipmentCategories.Queries.GetByLabId;
using LabBooking.Application.Features.EquipmentCategories.Queries.GetEquipments;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentCategoriesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách loại thiết bị (có phân trang, tìm kiếm, sắp xếp).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin, Manager, Lecturer, Student")]
    [ProducesResponseType(typeof(PagedResult<EquipmentCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllEquipmentCategoriesQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách thiết bị thuộc về một loại cụ thể.
    /// </summary>
    /// <param name="id">ID của loại thiết bị (Category ID)</param>
    [HttpGet("{id:guid}/equipments")]
    [Authorize(Roles = "Admin, Manager, Lecturer, Student")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentSimpleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipmentsByCategory(Guid id)
    {
        var result = await mediator.Send(new GetEquipmentsByCategoryQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách loại thiết bị và thiết bị thuộc về một phòng Lab cụ thể.
    /// </summary>
    /// <param name="labId">ID của phòng Lab</param>
    [HttpGet("lab/{labId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLabId(Guid labId)
    {
        var result = await mediator.Send(new GetCategoriesByLabIdQuery(labId));
        return Ok(result);
    }

    /// <summary>
    /// Tạo mới một loại thiết bị.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin, Manager")] // Chỉ Admin/Manager mới được tạo
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentCategoryCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Cập nhật thông tin loại thiết bị.
    /// </summary>
    /// <param name="id">ID của loại thiết bị cần sửa</param>
    /// <param name="command">Thông tin mới</param>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEquipmentCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID trong URL không khớp với ID trong dữ liệu gửi lên.");
        }

        await mediator.Send(command);
        return NoContent(); // Trả về 204 khi update thành công
    }
}
