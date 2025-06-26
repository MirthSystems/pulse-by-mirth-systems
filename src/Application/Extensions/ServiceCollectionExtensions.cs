namespace Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Infrastructure.Services;

using Azure;

using Common.Options;

using global::Application.Domain.Entities;
using global::Application.Infrastructure.Data.Context;

using Microsoft.EntityFrameworkCore;
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
    public static IServiceCollection AddApplication(this IServiceCollection services, Action<ApplicationOptions> configuration)
    {
        var config = new ApplicationOptions();
        configuration(config);

        // Register configuration
        services.AddSingleton(config);

        // Configure logging with Serilog
        ConfigureSerilog(services);

        // Register NodaTime IClock
        services.AddSingleton<IClock>(SystemClock.Instance);

        // Register application services
        RegisterApplicationServices(services);
        RegisterInfrastructureServices(services, config);

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

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        // Register application services
        services.AddScoped<Application.Common.Interfaces.Services.IVenueService, Application.Infrastructure.Services.VenueService>();
        services.AddScoped<Application.Common.Interfaces.Services.ISpecialService, Application.Infrastructure.Services.SpecialService>();
        
        // TODO: Add more services as they are implemented
        // services.AddScoped<INotificationService, NotificationService>();
        // services.AddScoped<IPostService, PostService>();
        // services.AddScoped<IActivityThreadService, ActivityThreadService>();
    }

    private static void RegisterInfrastructureServices(IServiceCollection services, ApplicationOptions config)
    {
        // Register repositories
        RegisterRepositories(services);

        // TODO: Register other infrastructure services here
        // services.AddScoped<IFileStorageService, LocalFileStorageService>();
        // services.AddScoped<INotificationService, SignalRNotificationService>();

        if (string.IsNullOrWhiteSpace(config.AzureMapsKey))
        {
            throw new ArgumentException("Azure Maps key must be provided in the configuration.", nameof(config.AzureMapsKey));
        }

        var azureMapsKeyCredential = new AzureKeyCredential(config.AzureMapsKey);
        if (azureMapsKeyCredential != null)
        {
            services.AddScoped<IAzureMapsService>(serviceProvider => new AzureMapsService(azureMapsKeyCredential));
        }
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // Register specific repositories with their interfaces
        services.AddScoped<Application.Common.Interfaces.Repositories.IVenueRepository, Application.Infrastructure.Data.Repositories.VenueRepository>();
        services.AddScoped<Application.Common.Interfaces.Repositories.ISpecialRepository, Application.Infrastructure.Data.Repositories.SpecialRepository>();
        services.AddScoped<Application.Common.Interfaces.Repositories.IVenueCategoryRepository, Application.Infrastructure.Data.Repositories.VenueCategoryRepository>();
        services.AddScoped<Application.Common.Interfaces.Repositories.ISpecialCategoryRepository, Application.Infrastructure.Data.Repositories.SpecialCategoryRepository>();

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
