namespace LabBooking.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Command handler responsible for processing the <see cref="RegisterUserCommand"/>.
/// </summary>
public class RegisterUserCommandHandler(
    ILogger<RegisterUserCommandHandler> logger,
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IMapper mapper
) : IRequestHandler<RegisterUserCommand, Unit>
{
    private const string DefaultRole = "SecurityGuard";

    public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing registration for email: {Email}", request.Email);

        // 1. Check if the "SecurityGuard" role exists BEFORE creating the user (Safety Check)
        var roleExists = await roleManager.RoleExistsAsync(DefaultRole);
        if (!roleExists)
        {
            logger.LogError("CRITICAL: Default role '{DefaultRole}' does not exist in the database.", DefaultRole);
            throw new Exception($"Cannot register. System configuration error: Role '{DefaultRole}' not found.");
        }

        // 2. Create the User object
        var newUser = mapper.Map<User>(request);

        // 3. Create the User in the database
        var createResult = await userManager.CreateAsync(newUser, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            logger.LogWarning("User creation failed for {Email}: {Errors}", request.Email, errors);
            throw new ValidationException(errors);
        }

        // 4. Assign the Default Role
        var roleResult = await userManager.AddToRoleAsync(newUser, DefaultRole);

        if (!roleResult.Succeeded)
        {
            var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            logger.LogError("User {Email} created successfully, BUT assigning role '{DefaultRole}' failed: {Errors}", request.Email, DefaultRole, roleErrors);
            await userManager.DeleteAsync(newUser);
            throw new Exception($"User created successfully but role assignment failed: {roleErrors}");
        }

        logger.LogInformation("User {Email} created and assigned role '{DefaultRole}' successfully.", request.Email, DefaultRole);
        return Unit.Value;
    }
}
