using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Authorization.Permissions;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for user venue permission operations
/// </summary>
public class UserVenuePermissionRepository : BaseRepository<UserVenuePermissionEntity, long>, IUserVenuePermissionRepository
{
    public UserVenuePermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserVenuePermissionEntity>> GetUserPermissionsAsync(
        long userId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query
            .Include(uvp => uvp.Venue)
            .Include(uvp => uvp.GrantedByUser)
            .Where(uvp => uvp.UserId == userId);

        if (activeOnly)
        {
            query = query.Where(uvp => uvp.IsActive);
        }

        return await query
            .OrderBy(uvp => uvp.Venue.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserVenuePermissionEntity>> GetVenuePermissionsAsync(
        long venueId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query
            .Include(uvp => uvp.User)
            .Include(uvp => uvp.GrantedByUser)
            .Where(uvp => uvp.VenueId == venueId);

        if (activeOnly)
        {
            query = query.Where(uvp => uvp.IsActive);
        }

        return await query
            .OrderBy(uvp => uvp.Name == VenuePermissions.Owner ? 0 : 1) // Owners first
            .ThenBy(uvp => uvp.User.Email)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserVenuePermissionEntity?> GetUserVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(uvp => uvp.Venue)
            .Include(uvp => uvp.User)
            .Include(uvp => uvp.GrantedByUser)
            .FirstOrDefaultAsync(uvp => uvp.UserId == userId && uvp.VenueId == venueId && uvp.IsActive, cancellationToken);
    }

    /// <summary>
    /// Checks if a user has ANY active permission for a venue (coarse-grained access check)
    /// </summary>
    /// <param name="userId">The user ID to check</param>
    /// <param name="venueId">The venue ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has any venue permission, false otherwise</returns>
    public async Task<bool> HasVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .AnyAsync(uvp => uvp.UserId == userId && uvp.VenueId == venueId && uvp.IsActive, cancellationToken);
    }

    /// <summary>
    /// Checks if a user has a SPECIFIC permission for a venue (fine-grained access check)
    /// </summary>
    /// <param name="userId">The user ID to check</param>
    /// <param name="venueId">The venue ID to check</param>
    /// <param name="permissionName">The specific permission name (e.g., "owner", "manager", "staff")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has the specific permission, false otherwise</returns>
    public async Task<bool> HasVenuePermissionAsync(
        long userId,
        long venueId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .AnyAsync(uvp => uvp.UserId == userId && uvp.VenueId == venueId && uvp.Name == permissionName && uvp.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<long>> GetUserVenueIdsAsync(
        long userId,
        string? permissionName = null,
        CancellationToken cancellationToken = default)
    {
        var query = Query
            .Where(uvp => uvp.UserId == userId && uvp.IsActive);

        if (!string.IsNullOrEmpty(permissionName))
        {
            query = query.Where(uvp => uvp.Name == permissionName);
        }

        return await query
            .Select(uvp => uvp.VenueId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserVenuePermissionEntity> GrantVenuePermissionAsync(
        long userId,
        long venueId,
        string permissionName,
        long grantedByUserId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        // Check if permission already exists
        var existingPermission = await Query
            .FirstOrDefaultAsync(uvp => uvp.UserId == userId && uvp.VenueId == venueId, cancellationToken);

        if (existingPermission != null)
        {
            // Update existing permission
            existingPermission.Name = permissionName;
            existingPermission.IsActive = true;
            existingPermission.GrantedByUserId = grantedByUserId;
            existingPermission.GrantedAt = SystemClock.Instance.GetCurrentInstant();
            existingPermission.Notes = notes;

            await UpdateAsync(existingPermission, cancellationToken);
            return existingPermission;
        }

        // Create new permission
        var newPermission = new UserVenuePermissionEntity
        {
            UserId = userId,
            VenueId = venueId,
            Name = permissionName,
            GrantedByUserId = grantedByUserId,
            GrantedAt = SystemClock.Instance.GetCurrentInstant(),
            IsActive = true,
            Notes = notes
        };

        return await AddAsync(newPermission, cancellationToken);
    }

    public async Task<bool> RevokeVenuePermissionAsync(
        long userId,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        var permission = await Query
            .FirstOrDefaultAsync(uvp => uvp.UserId == userId && uvp.VenueId == venueId && uvp.IsActive, cancellationToken);

        if (permission == null)
        {
            return false;
        }

        permission.IsActive = false;
        await UpdateAsync(permission, cancellationToken);
        return true;
    }
}
