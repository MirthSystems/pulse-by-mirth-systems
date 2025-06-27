using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Application.Infrastructure.Data.Context;

namespace Application.Services.DatabaseMigrations;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<ApplicationDbContext>("application-db", configureDbContextOptions: options =>
        {
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                npgsqlOptions.UseNodaTime();
                npgsqlOptions.UseNetTopologySuite();
            })
            .UseSnakeCaseNamingConvention();

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}
