namespace LabBooking.Infrastructure.Services.Authentication.Token;

public class JwtService(IConfiguration config) : IJwtService
{
    public RefreshTokenData GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        var expiryDays = config.GetValue("Jwt:RefreshTokenExpiryDays", 7);
        var expires = DateTime.UtcNow.AddDays(expiryDays);

        return new RefreshTokenData(token, expires);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwt = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
