using LabBooking.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LabBooking.Infrastructure.Implements.Common;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null) return [];

            return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
        }
    }
}
