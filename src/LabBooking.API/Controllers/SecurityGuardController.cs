using LabBooking.Application.Features.SecurityGuards.Commands.DeleteSecurityGuard;
using LabBooking.Application.Features.SecurityGuards.Commands.Register;
using LabBooking.Application.Features.SecurityGuards.Commands.UpdateSecurityGuard;
using LabBooking.Application.Features.SecurityGuards.Dtos;
using LabBooking.Application.Features.SecurityGuards.Queries.GetByIdSecurityGuard;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SecurityGuardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// Creates a new user and automatically assigns the "User" role.
    /// </remarks>
    /// <param name="command">Registration details (Email, Password, FullName...).</param>
    /// <returns>Successful account creation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSecurityGuard([FromBody] CreateSecurityGuardCommand command)
    {
        await mediator.Send(command);
        return CreatedAtAction(null, null, new { message = "Security Guard created successfully." });
    }

    /// <summary>
    /// Get security guard details by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SecurityGuardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SecurityGuardResponse>> GetById(Guid id)
    {
        var result = await mediator.Send(new GetSecurityGuardByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Update security guard information
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSecurityGuardCommand command)
    {
        if (id != command.Id && command.Id != Guid.Empty)
        {
            return BadRequest("Id mismatch");
        }

        command.Id = id; // Đảm bảo Id đúng
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a security guard
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteSecurityGuardCommand(id));
        return NoContent();
    }
}
