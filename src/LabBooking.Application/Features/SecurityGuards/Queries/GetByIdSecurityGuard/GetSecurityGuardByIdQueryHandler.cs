using LabBooking.Application.Features.SecurityGuards.Dtos;

namespace LabBooking.Application.Features.SecurityGuards.Queries.GetByIdSecurityGuard
{
    public class GetSecurityGuardByIdQueryHandler(
    UserManager<User> userManager,
    IMapper mapper
) : IRequestHandler<GetSecurityGuardByIdQuery, SecurityGuardResponse>
    {
        public async Task<SecurityGuardResponse> Handle(GetSecurityGuardByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Tìm User
            var user = await userManager.FindByIdAsync(request.Id.ToString());

            // 2. Kiểm tra tồn tại
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.Id.ToString());
            }

            // 3. Quan trọng: Kiểm tra user này có phải là SecurityGuard không?
            var isSecurityGuard = await userManager.IsInRoleAsync(user, "SecurityGuard");
            if (!isSecurityGuard)
            {
                // Nếu tìm thấy user nhưng không phải bảo vệ, coi như không tìm thấy (để bảo mật)
                throw new NotFoundException(nameof(User), request.Id.ToString());
            }

            // 4. Map sang DTO
            return mapper.Map<SecurityGuardResponse>(user);
        }
    }
}
