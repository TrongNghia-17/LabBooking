namespace LabBooking.Infrastructure.Services.Authentication.Token;

public class ClaimsGenerator(UserManager<User> userManager) : IClaimsGenerator
{
    public async Task<List<Claim>> GenerateClaimsAsync(User user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("AspNet.Identity.SecurityStamp", user.SecurityStamp!)
        };

        foreach (var role in userRoles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }
}
