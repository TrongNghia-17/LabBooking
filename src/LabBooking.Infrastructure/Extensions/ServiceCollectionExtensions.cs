namespace LabBooking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        var connectionString = configuration.GetConnectionString("LabBookingDb");
        services.AddDbContext<LabBookingDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                   .UseLazyLoadingProxies(false);

            if (isDevelopment)
                options.EnableSensitiveDataLogging();
        });

        services.AddIdentityApiEndpoints<User>(options =>
        {
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ "; // <-- Đã thêm khoảng trắng ở cuối
        })
        .AddRoles<IdentityRole<Guid>>()
        //.AddClaimsPrincipalFactory<LabsUserClaimsPrincipalFactory>()
        .AddEntityFrameworkStores<LabBookingDbContext>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddScoped<ILabBookingSeeder, LabBookingSeeder>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ICachingService, CachingService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IRefreshTokenFactory, RefreshTokenFactory>();
        services.AddScoped<IClaimsGenerator, ClaimsGenerator>();
    }
}
