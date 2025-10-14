using LabBooking.Application.Features.Auths;
using LabBooking.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LabBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly GoogleAuthService _googleAuthService;
    private readonly JwtService _jwtService;

    public AuthController(GoogleAuthService googleAuthService, JwtService jwtService)
    {
        _googleAuthService = googleAuthService;
        _jwtService = jwtService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest("Missing idToken.");

        var user = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
        if (user == null)
            return Unauthorized("Invalid Google token");

        // Kiểm tra domain email
        if (!user.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            return Forbid("Email domain is not allowed.");

        // Tạo JWT
        var jwt = _jwtService.GenerateToken(user.Email, user.Name);

        return Ok(new
        {
            access_token = jwt,
            user
        });
    }

    [HttpGet("profile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Profile()
    {
        var email = User.Claims.FirstOrDefault(c =>
        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

        var name = User.Claims.FirstOrDefault(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        return Ok(new { email, name });
    }
}
