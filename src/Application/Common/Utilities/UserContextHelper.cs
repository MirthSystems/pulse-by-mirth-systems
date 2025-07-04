using System.Security.Claims;
using Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Utilities;

/// <summary>
/// Utility class for extracting user information from HTTP context
/// </summary>
public static class UserContextHelper
{
    /// <summary>
    /// Gets the user's subject identifier from claims
    /// </summary>
    public static string? GetUserSub(ClaimsPrincipal user)
    {
        return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
               user.FindFirst(Application.Common.Constants.ClaimTypes.UserSub)?.Value;
    }

    /// <summary>
    /// Gets the user's email from claims using comprehensive claim lookup
    /// </summary>
    public static string? GetUserEmail(ClaimsPrincipal user, string? audience = null)
    {
        // Construct audience-namespaced email claim if audience is provided
        var audienceEmailClaim = !string.IsNullOrEmpty(audience) ? $"{audience}/email" : null;
        
        // Try common email claim types, including the audience-namespaced claim
        return user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
               ?? user.FindFirst("email")?.Value
               ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
               ?? user.FindFirst("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
               ?? (audienceEmailClaim != null ? user.FindFirst(audienceEmailClaim)?.Value : null)
               ?? user.Claims.FirstOrDefault(c => c.Type.EndsWith("/email"))?.Value;
    }

    /// <summary>
    /// Gets the user's name from claims
    /// </summary>
    public static string? GetUserName(ClaimsPrincipal user)
    {
        return user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ??
               user.FindFirst(Application.Common.Constants.ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Checks if user has a specific permission
    /// </summary>
    public static bool HasPermission(ClaimsPrincipal user, string permission)
    {
        return user.HasClaim(Application.Common.Constants.ClaimTypes.Permissions, permission);
    }

    /// <summary>
    /// Checks if user is a system administrator
    /// </summary>
    public static bool IsSystemAdmin(ClaimsPrincipal user)
    {
        return HasPermission(user, Auth0Permissions.SystemAdmin);
    }

    /// <summary>
    /// Checks if user is a content manager
    /// </summary>
    public static bool IsContentManager(ClaimsPrincipal user)
    {
        return HasPermission(user, Auth0Permissions.ContentManager);
    }

    /// <summary>
    /// Checks if user has backoffice access (system admin OR content manager)
    /// </summary>
    public static bool HasBackofficeAccess(ClaimsPrincipal user)
    {
        return IsSystemAdmin(user) || IsContentManager(user);
    }

    /// <summary>
    /// Gets all permissions for the user
    /// </summary>
    public static IEnumerable<string> GetPermissions(ClaimsPrincipal user)
    {
        return user.FindAll(Application.Common.Constants.ClaimTypes.Permissions).Select(c => c.Value);
    }

    /// <summary>
    /// Gets all roles for the user
    /// </summary>
    public static IEnumerable<string> GetRoles(ClaimsPrincipal user)
    {
        return user.FindAll(Application.Common.Constants.ClaimTypes.Roles).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if the user is authenticated
    /// </summary>
    public static bool IsAuthenticated(ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Gets user information as a dictionary for logging
    /// </summary>
    public static Dictionary<string, object?> GetUserInfo(ClaimsPrincipal user)
    {
        return new Dictionary<string, object?>
        {
            { "UserSub", GetUserSub(user) },
            { "Email", GetUserEmail(user) },
            { "Name", GetUserName(user) },
            { "IsAuthenticated", IsAuthenticated(user) },
            { "IsSystemAdmin", IsSystemAdmin(user) },
            { "IsContentManager", IsContentManager(user) },
            { "Permissions", GetPermissions(user) },
            { "Roles", GetRoles(user) }
        };
    }
}
