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
    /// Lấy danh sách tất cả các loại thiết bị (Categories).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin, Manager, Lecturer, Student")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllEquipmentCategoriesQuery());
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
}
