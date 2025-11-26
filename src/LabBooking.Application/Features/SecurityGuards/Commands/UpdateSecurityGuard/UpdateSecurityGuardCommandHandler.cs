namespace LabBooking.Application.Features.SecurityGuards.Commands.UpdateSecurityGuard
{
    public class UpdateSecurityGuardCommandHandler(
    UserManager<User> userManager
) : IRequestHandler<UpdateSecurityGuardCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateSecurityGuardCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id.ToString());

            if (user == null || !await userManager.IsInRoleAsync(user, "SecurityGuard"))
            {
                throw new NotFoundException(nameof(User), request.Id.ToString());
            }

            // Cập nhật thông tin
            user.Email = request.Email;
            user.UserName = request.Email; // Thường UserName = Email
            user.PhoneNumber = request.PhoneNumber;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors); // Giả định có ValidationException custom
            }

            return Unit.Value;
        }
    }
}
