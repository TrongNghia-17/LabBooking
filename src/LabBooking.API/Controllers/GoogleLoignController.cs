namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleLoignController(IMediator mediator) : ControllerBase
{
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var authResponse = await mediator.Send(command);

        //SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.RefreshTokenExpiry);

        return Ok(new
        {
            authResponse.AccessToken,
            authResponse.RefreshToken,
            authResponse.RefreshTokenExpiry
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest requestBody)
    {
        //var refreshToken = Request.Cookies["refreshToken"];
        var refreshToken = requestBody?.RefreshToken;
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "No refresh token provided." });

        var command = new RefreshTokenCommand(refreshToken);
        var response = await mediator.Send(command);

        //SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiry);

        return Ok(new
        {
            response.AccessToken,
            response.RefreshToken,
            response.RefreshTokenExpiry

        });
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);
        return Ok(new { email, name });
    }

    private void SetRefreshTokenCookie(string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
