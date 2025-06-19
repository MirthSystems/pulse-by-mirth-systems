namespace Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using global::Application.Domain.Entities;
using global::Application.Infrastructure.Data.Context;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Serilog;
using Serilog.Events;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds PulseByMirthSystems services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        // Configure logging with Serilog
        ConfigureSerilog(services);

        // Register NodaTime IClock
        services.AddSingleton<IClock>(SystemClock.Instance);

        // Configure ASP.NET Core Identity
        ConfigureIdentity(services);

        // Register application services
        RegisterApplicationServices(services);
        RegisterInfrastructureServices(services);

        // Configure SignalR
        ConfigureSignalR(services);

        return services;
    }

    private static void ConfigureSerilog(IServiceCollection services)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}");

        Log.Logger = loggerConfiguration.CreateLogger();
        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Cookie configuration will be done in the consuming application
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        // Register application services
        // TODO: Add more services as they are implemented
        // services.AddScoped<INotificationService, NotificationService>();
        // services.AddScoped<IPostService, PostService>();
        // services.AddScoped<IActivityThreadService, ActivityThreadService>();
    }

    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        // Register repositories
        RegisterRepositories(services);

        // TODO: Register other infrastructure services here
        // services.AddScoped<IFileStorageService, LocalFileStorageService>();
        // services.AddScoped<INotificationService, SignalRNotificationService>();

        // Azure Maps services (if key provided)
        //if (!string.IsNullOrWhiteSpace(config.AzureMapsKey))
        //{
        //    // TODO: Register Azure Maps services
        //    // services.AddScoped<IGeocodingService, AzureMapsGeocodingService>();
        //}
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // Register specific repositories with their interfaces

        // Note: BaseRepository<,> is abstract and should not be registered directly.
        // Only concrete implementations should be registered with the DI container.
    }

    private static void ConfigureSignalR(IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        });
    }
}
