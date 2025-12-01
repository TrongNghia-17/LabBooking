using LabBooking.Application.Services.Notifications;
using LabBooking.Application.Services.Users;
using LabBooking.Infrastructure.Services.Notifications;
using LabBooking.Infrastructure.Services.Users;


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
            options.UseNpgsql(connectionString);
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            //.UseLazyLoadingProxies(false);

            if (isDevelopment)
                options.EnableSensitiveDataLogging();
        });

        services.AddIdentityCore<User>(options =>
        {
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<LabBookingDbContext>()
        .AddDefaultTokenProviders();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddScoped<ILabBookingSeeder, LabBookingSeeder>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ISupportRepository, SupportRepository>();
        services.AddScoped<ILabRoomRepository, LabRoomRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IEquipmentMaintainScheduleRepository, EquipmentMaintainScheduleRepository>();
        services.AddScoped<IEquipmentCategoryRepository, EquipmentCategoryRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ISlotRepository, SlotRepository>();
        services.AddScoped<IBookingSlotRepository, BookingSlotRepository>();
        services.AddScoped<IRoomMaintainScheduleRepository, RoomMaintainScheduleRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IBookingChangeRequestRepository, BookingChangeRequestRepository>();
        services.AddScoped<IBookingConsentRequestRepository, BookingConsentRequestRepository>();

        services.AddScoped<IUsagePolicyRepository, UsagePolicyRepository>();

        services.AddScoped<ICachingService, CachingService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IRefreshTokenFactory, RefreshTokenFactory>();
        services.AddScoped<IClaimsGenerator, ClaimsGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpClient();
    }
}
