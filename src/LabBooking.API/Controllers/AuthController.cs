namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var authResponse = await mediator.Send(command);
        return Ok(authResponse);
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);
        return Ok(new { email, name });
    }
}
