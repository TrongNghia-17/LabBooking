using LabBooking.Application.Features.Authentication.Queries.GetProfile;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Logs in using a Google account.
    /// </summary>
    /// <param name="command">Contains the GoogleIdToken.</param>
    /// <returns>An Access Token and Refresh Token.</returns>
    [HttpPost("google")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var authResponse = await mediator.Send(command);
        return Ok(authResponse);
    }

    /// <summary>
    /// Logs in using Email and Password.
    /// </summary>
    /// <param name="command">Contains the Email and Password.</param>
    /// <returns>An Access Token and Refresh Token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserCommand command)
    {
        var authResponse = await mediator.Send(command);
        return Ok(authResponse);
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <remarks>
    /// Invalidates the refresh token and access token by updating the Security Stamp.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await mediator.Send(new LogoutUserCommand());
        return NoContent();
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// Creates a new user and automatically assigns the "User" role.
    /// </remarks>
    /// <param name="command">Registration details (Email, Password, FullName...).</param>
    /// <returns>Successful account creation.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        await mediator.Send(command);
        return CreatedAtAction(nameof(GetProfileAsync), null, new { message = "Đăng ký thành công." });
    }

    /// <summary>
    /// Refreshes the Access Token using a Refresh Token.
    /// </summary>
    /// <param name="requestBody">Contains the Refresh Token.</param>
    /// <returns>A new pair of Access Token and Refresh Token.</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest requestBody)
    {
        var refreshToken = requestBody!.RefreshToken;
        var command = new RefreshTokenCommand(refreshToken);
        var authResponse = await mediator.Send(command);

        return Ok(authResponse);
    }

    /// <summary>
    /// Gets the profile information of the currently authenticated user.
    /// </summary>
    /// <returns>The user's detailed information.</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetProfileAsync()
    {
        var query = new GetProfileQuery();
        var userProfile = await mediator.Send(query);

        return Ok(userProfile);
    }
}
