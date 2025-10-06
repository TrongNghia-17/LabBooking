namespace LabBooking.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(cfg => { }, applicationAssembly);

        services.AddValidatorsFromAssembly(applicationAssembly)
           .AddFluentValidationAutoValidation();
    }
}
