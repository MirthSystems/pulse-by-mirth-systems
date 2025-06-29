using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for user venue permission operations
/// </summary>
public interface IUserVenuePermissionRepository : IRepository<UserVenuePermissionEntity, long>
{
    /// <summary>
    /// Gets all venue permissions for a specific user
    /// </summary>
    Task<IEnumerable<UserVenuePermissionEntity>> GetUserPermissionsAsync(
        long userId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user permissions for a specific venue
    /// </summary>
    Task<IEnumerable<UserVenuePermissionEntity>> GetVenuePermissionsAsync(
        long venueId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific permission for a user and venue
    /// </summary>
    Task<UserVenuePermissionEntity?> GetUserVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has any permission for a venue
    /// </summary>
    Task<bool> HasVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission for a venue
    /// </summary>
    Task<bool> HasVenuePermissionAsync(
        long userId,
        long venueId,
        string permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all venue IDs that a user has access to
    /// </summary>
    Task<IEnumerable<long>> GetUserVenueIdsAsync(
        long userId,
        string? permissionName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new venue permission
    /// </summary>
    Task<UserVenuePermissionEntity> GrantVenuePermissionAsync(
        long userId,
        long venueId,
        string name,
        long grantedByUserId,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a venue permission by setting it inactive
    /// </summary>
    Task<bool> RevokeVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default);
}
