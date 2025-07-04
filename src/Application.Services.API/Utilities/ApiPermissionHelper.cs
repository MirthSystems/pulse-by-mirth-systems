using Application.Common.Utilities;
using System.Security.Claims;

namespace Application.Services.API.Utilities;

/// <summary>
/// API-specific utilities for permission checking without MVC dependencies
/// Keeps business logic in Common layer while API concerns stay in API layer
/// </summary>
public static class ApiPermissionHelper
{
    /// <summary>
    /// Gets accessible venue IDs for a user (wraps Common layer helper)
    /// </summary>
    public static async Task<IEnumerable<long>> GetAccessibleVenueIdsAsync(
        Application.Common.Interfaces.Services.IPermissionService permissionService,
        ClaimsPrincipal user,
        string userSub,
        CancellationToken cancellationToken = default)
    {
        return await PermissionUtils.GetAccessibleVenueIdsAsync(
            permissionService, user, userSub, cancellationToken);
    }

    /// <summary>
    /// Checks if user has access to a specific venue (wraps Common layer helper)
    /// </summary>
    public static async Task<bool> HasVenueAccessAsync(
        Application.Common.Interfaces.Services.IPermissionService permissionService,
        ClaimsPrincipal user,
        string userSub,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        return await PermissionUtils.HasVenueAccessAsync(
            permissionService, user, userSub, venueId, cancellationToken);
    }

    /// <summary>
    /// Checks if user has system-level permissions
    /// </summary>
    public static bool HasSystemPermissions(ClaimsPrincipal user)
    {
        return PermissionUtils.HasSystemPermissions(user);
    }

    /// <summary>
    /// Gets user sub from claims
    /// </summary>
    public static string? GetUserSub(ClaimsPrincipal user)
    {
        return UserContextUtils.GetUserSub(user);
    }
}
