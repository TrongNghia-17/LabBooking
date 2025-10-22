using LabBooking.Infrastructure.Services.Authentication;
using LabBooking.Infrastructure.Services.Caching;

namespace LabBooking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LabBookingDb");
        services.AddDbContext<LabBookingDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                   .UseLazyLoadingProxies(false);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
            }
        });

        services.AddIdentityApiEndpoints<User>()
                .AddRoles<IdentityRole<Guid>>()
                //.AddClaimsPrincipalFactory<LabsUserClaimsPrincipalFactory>()
                .AddEntityFrameworkStores<LabBookingDbContext>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IDoorRequestRepository, DoorRequestRepository>();

        services.AddScoped<ICachingService, CachingService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
    }
}
