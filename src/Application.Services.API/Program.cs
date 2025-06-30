using Application.Extensions;
using Application.Infrastructure.Data.Context;
using Application.Services.API.Middleware;
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
                    NameClaimType = ClaimTypes.NameIdentifier,
                    // Map Auth0 claims to standard claims
                    RoleClaimType = "permissions"
                };
                
                // Subscribe to events to handle additional claim mapping
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Ensure email claim is properly mapped
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            // Auth0 might send email in different claim types
                            var emailClaim = claimsIdentity.FindFirst("email") 
                                          ?? claimsIdentity.FindFirst("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                            
                            if (emailClaim != null && !claimsIdentity.HasClaim(ClaimTypes.Email, emailClaim.Value))
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, emailClaim.Value));
                            }
                            
                            // Ensure name claim mapping
                            var nameClaim = claimsIdentity.FindFirst("name") 
                                         ?? claimsIdentity.FindFirst("nickname")
                                         ?? claimsIdentity.FindFirst("given_name");
                            
                            if (nameClaim != null && !claimsIdentity.HasClaim(ClaimTypes.Name, nameClaim.Value))
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value));
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins", policy =>
            {
                var corsSection = builder.Configuration.GetSection("CORS");
                var allowedOrigins = corsSection.GetSection("AllowedOrigins")
                    .Get<string[]>() ?? Array.Empty<string>();
                var allowedMethods = corsSection.GetSection("AllowedMethods")
                    .Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH" };
                var allowedHeaders = corsSection.GetSection("AllowedHeaders")
                    .Get<string[]>() ?? new[] { "Content-Type", "Authorization", "X-Requested-With", "Accept", "Origin" };
                var allowCredentials = corsSection.GetValue<bool>("AllowCredentials", true);

                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .WithMethods(allowedMethods)
                          .WithHeaders(allowedHeaders);
                    
                    if (allowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                }
                else
                {
                    // Fallback for development - never use in production
                    policy.AllowAnyOrigin()
                          .WithMethods(allowedMethods)
                          .WithHeaders(allowedHeaders);
                }
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

        builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Application.Infrastructure.Authorization.Handlers.BackofficeAuthorizationHandler>();

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

        // Add CORS debugging in development
        if (app.Environment.IsDevelopment())
        {
            app.UseCorsDebugging();
        }

        app.UseCors("AllowedOrigins");
        app.UseAuthentication();
        app.UseUserProvisioning(); // Ensure users exist in database after authentication
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
