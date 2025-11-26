namespace LabBooking.Application.Features.SecurityGuards.Commands.DeleteSecurityGuard
{
    public class DeleteSecurityGuardCommandHandler(
    UserManager<User> userManager
) : IRequestHandler<DeleteSecurityGuardCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteSecurityGuardCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id.ToString());

            if (user == null || !await userManager.IsInRoleAsync(user, "SecurityGuard"))
            {
                throw new NotFoundException(nameof(User), request.Id.ToString());
            }

            // Thực hiện xóa (Hard delete)
            // Nếu muốn Soft delete (ẩn đi), bạn cần update property IsActive = false thay vì gọi DeleteAsync
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to delete security guard.");
            }

            return Unit.Value;
        }
    }
}
