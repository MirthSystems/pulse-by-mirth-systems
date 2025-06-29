using Application.Common.Models.Auth;
using Application.Domain.Entities;

namespace Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for managing venue permissions and invitations
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if user can manage venue (any permission level)
    /// </summary>
    Task<bool> CanUserManageVenueAsync(string userSub, long venueId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user is venue owner
    /// </summary>
    Task<bool> IsUserVenueOwnerAsync(string userSub, long venueId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user is venue staff (any non-owner permission)
    /// </summary>
    Task<bool> IsUserVenueStaffAsync(string userSub, long venueId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all venue IDs that user has access to
    /// </summary>
    Task<IEnumerable<long>> GetUserVenueIdsAsync(string userSub, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all venue IDs that user has access to (includes all venues for admins/content managers)
    /// </summary>
    Task<IEnumerable<long>> GetUserAccessibleVenueIdsAsync(string userSub, bool isSystemAdmin, bool isContentManager, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user's venue permissions summary
    /// </summary>
    Task<IEnumerable<VenuePermissionSummary>> GetUserVenuePermissionsAsync(string userSub, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invite user to venue with specific permission
    /// </summary>
    Task<VenueInvitationEntity> InviteUserToVenueAsync(
        string email, 
        long venueId, 
        string permission, 
        string invitedByUserSub,
        string? notes = null,
        int expirationDays = 7,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Accept venue invitation on user login
    /// </summary>
    Task<bool> AcceptVenueInvitationAsync(string userSub, string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove user from venue (revoke permission)
    /// </summary>
    Task<bool> RemoveUserFromVenueAsync(string userSub, long venueId, string removedByUserSub, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Grant permission to user for venue
    /// </summary>
    Task<UserVenuePermissionEntity> GrantVenuePermissionAsync(
        string userSub, 
        long venueId, 
        string permission, 
        string grantedByUserSub,
        string? notes = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ensure user exists in database and return user ID
    /// </summary>
    Task<long> EnsureUserExistsAsync(string userSub, string email, string? name = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user has any permission for a venue
    /// </summary>
    Task<bool> HasVenuePermissionAsync(string userSub, long venueId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all permissions for a specific venue
    /// </summary>
    Task<IEnumerable<UserVenuePermissionEntity>> GetVenuePermissionsAsync(long venueId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all invitations for a specific venue
    /// </summary>
    Task<IEnumerable<VenueInvitationEntity>> GetVenueInvitationsAsync(long venueId, CancellationToken cancellationToken = default);
}
