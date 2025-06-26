using Application.Extensions;
using Application.Infrastructure.Data.Context;

using Microsoft.OpenApi.Models;

namespace Application.Services.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache("cache");
        builder.AddNpgsqlDbContext<ApplicationDbContext>("application-db");

        builder.Services.AddProblemDetails();

        builder.Services.AddApplication(options =>
        {
            builder.Configuration.Bind("Application", options);
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Application API Server",
                Description = "Learn how to protect your .NET applications with Auth0",
                Contact = new OpenApiContact
                {
                    Name = ".NET Identity with Auth0",
                    Url = new Uri("https://a0.to/dotnet-templates/webapi")
                },
                Version = "v1.0.0"
            });

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                  {
                  { securitySchema, new[] { "Bearer" } }
                  });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
