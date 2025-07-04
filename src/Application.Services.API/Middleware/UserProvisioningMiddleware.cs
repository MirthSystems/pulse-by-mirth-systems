using Application.Common.Interfaces.Services;
using Application.Common.Utilities;
using System.Security.Claims;

namespace Application.Services.API.Middleware;

/// <summary>
/// Middleware to ensure authenticated users exist in the database with their email
/// This runs after authentication but before authorization to guarantee user consistency
/// </summary>
public class UserProvisioningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserProvisioningMiddleware> _logger;

    public UserProvisioningMiddleware(RequestDelegate next, ILogger<UserProvisioningMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, IPermissionService permissionService, IConfiguration configuration)
    {
        // Only process authenticated requests
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var userSub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userSub))
                {
                    // Use centralized email claim extraction with audience support
                    var audience = configuration["Auth0:Audience"];
                    var userEmail = UserContextHelper.GetUserEmail(context.User, audience);

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        // Ensure user exists in database - this is idempotent and safe to call multiple times
                        await permissionService.EnsureUserExistsAsync(userSub, userEmail, cancellationToken: context.RequestAborted);
                        _logger.LogDebug("User {UserSub} with email {Email} provisioned in database", userSub, userEmail);
                    }
                    else
                    {
                        // Check if user already exists in database
                        var existingEmail = await permissionService.GetUserEmailBySubAsync(userSub, context.RequestAborted);
                        if (string.IsNullOrEmpty(existingEmail))
                        {
                            // Log this case but don't fail the request - let individual endpoints handle it
                            var claims = context.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                            _logger.LogWarning("Authenticated user {UserSub} has no email in JWT claims and is not in database. Available claims: {Claims}", 
                                userSub, string.Join(", ", claims));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the request - individual endpoints can handle missing users
                _logger.LogError(ex, "Error provisioning user in database during middleware");
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to register the UserProvisioningMiddleware
/// </summary>
public static class UserProvisioningMiddlewareExtensions
{
    public static IApplicationBuilder UseUserProvisioning(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserProvisioningMiddleware>();
    }
}
