using Application.Extensions;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using NSwag;

namespace Application.Services.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache("cache");
        builder.AddNpgsqlDbContext<ApplicationDbContext>("application-db", configureDbContextOptions: options =>
        {
            options.UseNpgsql(npgsqlOptions =>
            {
                npgsqlOptions.UseNodaTime();
                npgsqlOptions.UseNetTopologySuite();
            })
            .UseSnakeCaseNamingConvention();
        });

        builder.Services.AddProblemDetails();

        builder.Services.AddApplication(options =>
        {
            builder.Configuration.Bind("Application", options);
        });

        // Configure Auth0 JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
                options.Audience = builder.Configuration["Auth0:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowVueApp", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5367", 
                        "https://localhost:5367",
                        "http://localhost:5173", 
                        "https://localhost:5173"
                      )
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddControllers();
        
        // Add NSwag services with comprehensive configuration
        builder.Services.AddOpenApiDocument(configure =>
        {
            configure.Title = "Pulse Application API";
            configure.Description = "Comprehensive API for Pulse venue and specials management with Auth0 authentication";
            configure.Version = "v1.0.0";
            
            // Add Auth0 JWT security scheme
            configure.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
            {
                Type = NSwag.OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your Auth0 JWT token. You can obtain this from the Auth0 login flow."
            });

            // Set up operation processors for automatic security requirements
            configure.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // NSwag OpenAPI documentation
            app.UseOpenApi(configure =>
            {
                configure.Path = "/api/openapi/{documentName}/openapi.json";
            });
            
            // NSwag Swagger UI (comprehensive documentation UI)
            app.UseSwaggerUi(configure =>
            {
                configure.Path = "/api/docs";
                configure.DocumentPath = "/api/openapi/{documentName}/openapi.json";
            });
            
            // NSwag ReDoc UI (alternative documentation UI)
            app.UseReDoc(configure =>
            {
                configure.Path = "/api/redoc";
                configure.DocumentPath = "/api/openapi/{documentName}/openapi.json";
            });
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowVueApp");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
