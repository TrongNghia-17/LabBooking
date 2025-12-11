using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.UsagePolicies.Commands.CreateUsagePolicy;
using LabBooking.Application.Features.UsagePolicies.Commands.DeleteUsagePolicy;
using LabBooking.Application.Features.UsagePolicies.Commands.UpdateUsagePolicy;
using LabBooking.Application.Features.UsagePolicies.Dtos;
using LabBooking.Application.Features.UsagePolicies.Queries.GetAllUsagePolicies;
using LabBooking.Application.Features.UsagePolicies.Queries.GetUsagePolicyById;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsagePoliciesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new usage policy
    /// </summary>
    /// <param name="command">The data for the new usage policy</param>
    /// <returns>The newly created usage policy</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateUsagePolicyCommand command)
    {
        var id = await mediator.Send(command);

        // Trả về 201 Created cùng với location của resource mới
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Update an existing usage policy
    /// (Tương tự LabRoomsController)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUsagePolicyCommand command) // Giả định bạn đã tạo UpdateUsagePolicyCommand
    {
        command.Id = id; // Gán Id từ route vào command
        await mediator.Send(command);

        return NoContent(); //
    }

    /// <summary>
    /// Get a specific usage policy by Id
    /// </summary>
    /// <param name="id">The Id of the policy</param>
    /// <returns>The usage policy</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")] // Giống LabRoomsController
    [ProducesResponseType(typeof(UsagePolicyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UsagePolicyResponse>> GetById([FromRoute] Guid id)
    {
        // Sử dụng Query chúng ta đã tạo ở bước trước
        var query = new GetUsagePolicyByIdQuery(id);
        var policy = await mediator.Send(query);

        return Ok(policy);
    }

    /// <summary>
    /// Get all usage policies with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering policies</param>
    /// <returns>List of usage policies</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")] // Giống LabRoomsController
    [ProducesResponseType(typeof(PagedResult<UsagePolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<UsagePolicyResponse>>> GetAll([FromQuery] GetAllUsagePoliciesQuery query)
    {
        var policies = await mediator.Send(query);
        return Ok(policies);
    }

    /// <summary>
    /// Delete an existing usage policy
    /// (Tương tự LabRoomsController)
    /// </summary>
    /// <param name="id">The Id of the policy to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteUsagePolicyCommand(id);
        await mediator.Send(command);

        return NoContent(); // Trả về 204 No Content
    }
}
