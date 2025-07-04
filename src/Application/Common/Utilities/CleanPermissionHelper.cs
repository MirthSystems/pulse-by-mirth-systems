using Application.Common.Interfaces.Services;
using Application.Common.Utilities;
using System.Security.Claims;

namespace Application.Common.Utilities;

/// <summary>
/// Clean permission validation helper without MVC dependencies
/// Contains only business logic for permission checking
/// </summary>
public static class CleanPermissionHelper
{
    /// <summary>
    /// Checks if user has venue access (system admin, content manager, or venue-specific permission)
    /// </summary>
    public static async Task<bool> HasVenueAccessAsync(
        IPermissionService permissionService,
        ClaimsPrincipal user,
        string userSub,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        // System-level permissions bypass venue-specific checks
        if (HasSystemPermissions(user))
        {
            return true;
        }
        
        // Check venue-specific permissions
        return await permissionService.HasVenuePermissionAsync(userSub, venueId, cancellationToken);
    }
    
    /// <summary>
    /// Gets accessible venue IDs for a user based on their permissions
    /// </summary>
    public static async Task<IEnumerable<long>> GetAccessibleVenueIdsAsync(
        IPermissionService permissionService,
        ClaimsPrincipal user,
        string userSub,
        CancellationToken cancellationToken = default)
    {
        var isSystemAdmin = UserContextHelper.IsSystemAdmin(user);
        var isContentManager = UserContextHelper.IsContentManager(user);
        
        return await permissionService.GetUserAccessibleVenueIdsAsync(
            userSub, isSystemAdmin, isContentManager, cancellationToken);
    }

    /// <summary>
    /// Checks if user has system-level permissions (admin or content manager)
    /// </summary>
    public static bool HasSystemPermissions(ClaimsPrincipal user)
    {
        return UserContextHelper.IsSystemAdmin(user) || UserContextHelper.IsContentManager(user);
    }

    /// <summary>
    /// Validates that user can create venues (system admin or content manager only)
    /// </summary>
    public static bool CanCreateVenues(ClaimsPrincipal user)
    {
        return HasSystemPermissions(user);
    }
}
