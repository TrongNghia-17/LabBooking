namespace LabBooking.Application.Features.Users.Commands.AssignRole;

public class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AssignRoleToUserCommandValidator(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID không được để trống.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role Name không được để trống.")
            .MustAsync(RoleMustExist)
            .WithMessage(cmd => $"Role '{cmd.RoleName}' không tồn tại trong hệ thống.");
    }

    /// <summary>
    /// Kiểm tra xem Role có tồn tại trong CSDL hay không
    /// </summary>
    private async Task<bool> RoleMustExist(string roleName, CancellationToken token)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }
}
