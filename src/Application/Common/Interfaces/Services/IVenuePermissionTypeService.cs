namespace Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for managing venue permission types
/// </summary>
public interface IVenuePermissionTypeService
{
    /// <summary>
    /// Get all valid permission types
    /// </summary>
    Task<IEnumerable<string>> GetValidPermissionTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate if a permission type is valid
    /// </summary>
    Task<bool> IsValidPermissionTypeAsync(string permissionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get display name for a permission type
    /// </summary>
    Task<string> GetPermissionDisplayNameAsync(string permissionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get permission hierarchy (what permissions this type includes)
    /// </summary>
    Task<string[]> GetPermissionHierarchyAsync(string permissionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidate permission types cache
    /// </summary>
    void InvalidateCache();
}
