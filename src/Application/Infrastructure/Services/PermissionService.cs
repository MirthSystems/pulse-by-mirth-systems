using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Models.Auth;
using Application.Domain.Entities;
using Application.Infrastructure.Authorization.Permissions;
using Microsoft.Extensions.Logging;
using NodaTime;
using System.Linq;

namespace Application.Infrastructure.Services;

/// <summary>
/// Service implementation for managing venue permissions and invitations
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUserRepository _userRepository;    
    private readonly IUserVenuePermissionRepository _permissionRepository;
    private readonly IVenueInvitationRepository _invitationRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IClock _clock;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IUserRepository userRepository,
        IUserVenuePermissionRepository permissionRepository,
        IVenueInvitationRepository invitationRepository,
        IVenueRepository venueRepository,
        IClock clock,
        ILogger<PermissionService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string?> GetUserEmailBySubAsync(string userSub, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            return user?.Email;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user email for user {UserSub}", userSub);
            return null;
        }
    }

    public async Task<bool> CanUserManageVenueAsync(string userSub, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            return await _permissionRepository.HasVenuePermissionAsync(user.Id, venueId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserSub} can manage venue {VenueId}", userSub, venueId);
            return false;
        }
    }

    public async Task<bool> IsUserVenueOwnerAsync(string userSub, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            return await _permissionRepository.HasVenuePermissionAsync(user.Id, venueId, VenuePermissions.Owner, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserSub} is owner of venue {VenueId}", userSub, venueId);
            return false;
        }
    }

    public async Task<bool> IsUserVenueStaffAsync(string userSub, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            return await _permissionRepository.HasVenuePermissionAsync(user.Id, venueId, VenuePermissions.Manager, cancellationToken) ||
                   await _permissionRepository.HasVenuePermissionAsync(user.Id, venueId, VenuePermissions.Staff, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserSub} is staff of venue {VenueId}", userSub, venueId);
            return false;
        }
    }

    public async Task<IEnumerable<long>> GetUserVenueIdsAsync(string userSub, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return Array.Empty<long>();

            return await _permissionRepository.GetUserVenueIdsAsync(user.Id, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue IDs for user {UserSub}", userSub);
            return Array.Empty<long>();
        }
    }

    public async Task<IEnumerable<VenuePermissionSummary>> GetUserVenuePermissionsAsync(string userSub, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return Array.Empty<VenuePermissionSummary>();

            var permissions = await _permissionRepository.GetUserPermissionsAsync(user.Id, activeOnly: true, cancellationToken);

            return permissions.Select(p => new VenuePermissionSummary
            {
                Id = p.Id,
                UserId = p.UserId,
                VenueId = p.VenueId,
                VenueName = p.Venue.Name,
                Name = p.Name,
                GrantedByUserId = p.GrantedByUserId,
                GrantedByName = p.GrantedByUser.Email,
                GrantedAt = p.GrantedAt.ToDateTimeUtc(),
                IsActive = p.IsActive,
                Notes = p.Notes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue permissions for user {UserSub}", userSub);
            return Array.Empty<VenuePermissionSummary>();
        }
    }

    public async Task<VenueInvitationEntity> InviteUserToVenueAsync(
        string email,
        long venueId,
        string permission,
        string invitedByUserSub,
        string? notes = null,
        int expirationDays = 7,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate permission
            if (!VenuePermissions.IsValidPermission(permission))
            {
                throw new ArgumentException($"Invalid permission: {permission}", nameof(permission));
            }

            // Get inviting user
            var invitingUser = await _userRepository.GetBySubAsync(invitedByUserSub, cancellationToken);
            if (invitingUser == null)
            {
                throw new InvalidOperationException($"Inviting user not found: {invitedByUserSub}");
            }

            // Verify venue exists
            var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
            if (venue == null)
            {
                throw new InvalidOperationException($"Venue not found: {venueId}");
            }

            // Check if invitation already exists
            var existingInvitation = await _invitationRepository.HasPendingInvitationAsync(email, venueId, cancellationToken);
            if (existingInvitation)
            {
                throw new InvalidOperationException($"Pending invitation already exists for {email} to venue {venueId}");
            }

            // Create invitation
            var invitation = await _invitationRepository.CreateInvitationAsync(
                email, venueId, permission, invitingUser.Id, expirationDays, notes, cancellationToken);

            _logger.LogInformation("User {InvitedByUserSub} invited {Email} to venue {VenueId} with permission {Permission}",
                invitedByUserSub, email, venueId, permission);

            return invitation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inviting user {Email} to venue {VenueId}", email, venueId);
            throw;
        }
    }

    public async Task<bool> AcceptVenueInvitationAsync(string userSub, string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            var pendingInvitations = await _invitationRepository.GetPendingInvitationsAsync(email, cancellationToken);
            
            bool hasAcceptedAny = false;
            foreach (var invitation in pendingInvitations.Where(i => i.IsValid))
            {
                var result = await _invitationRepository.AcceptInvitationAsync(invitation.Id, user.Id, cancellationToken);
                if (result != null)
                {
                    hasAcceptedAny = true;
                    _logger.LogInformation("User {UserSub} accepted invitation to venue {VenueId} with permission {Permission}",
                        userSub, invitation.VenueId, invitation.Permission);
                }
            }

            return hasAcceptedAny;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting venue invitations for user {UserSub}", userSub);
            return false;
        }
    }

    public async Task<bool> RemoveUserFromVenueAsync(string userSub, long venueId, string removedByUserSub, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            var result = await _permissionRepository.RevokeVenuePermissionAsync(user.Id, venueId, cancellationToken);
            
            if (result)
            {
                _logger.LogInformation("User {UserSub} removed from venue {VenueId} by {RemovedByUserSub}",
                    userSub, venueId, removedByUserSub);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserSub} from venue {VenueId}", userSub, venueId);
            return false;
        }
    }

    public async Task<UserVenuePermissionEntity> GrantVenuePermissionAsync(
        string userSub,
        long venueId,
        string permission,
        string grantedByUserSub,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate permission
            if (!VenuePermissions.IsValidPermission(permission))
            {
                throw new ArgumentException($"Invalid permission: {permission}", nameof(permission));
            }

            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User not found: {userSub}");
            }

            var grantingUser = await _userRepository.GetBySubAsync(grantedByUserSub, cancellationToken);
            if (grantingUser == null)
            {
                throw new InvalidOperationException($"Granting user not found: {grantedByUserSub}");
            }

            var result = await _permissionRepository.GrantVenuePermissionAsync(
                user.Id, venueId, permission, grantingUser.Id, notes, cancellationToken);

            _logger.LogInformation("User {UserSub} granted {Permission} permission to venue {VenueId} by {GrantedByUserSub}",
                userSub, permission, venueId, grantedByUserSub);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting venue permission to user {UserSub}", userSub);
            throw;
        }
    }

    public async Task<long> EnsureUserExistsAsync(string userSub, string email, string? name = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.UpsertUserAsync(userSub, email, name, cancellationToken: cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring user exists for {UserSub}", userSub);
            throw;
        }
    }

    public async Task<bool> HasVenuePermissionAsync(string userSub, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetBySubAsync(userSub, cancellationToken);
            if (user == null) return false;

            return await _permissionRepository.HasVenuePermissionAsync(user.Id, venueId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserSub} has venue permission for {VenueId}", userSub, venueId);
            return false;
        }
    }

    public async Task<IEnumerable<long>> GetUserAccessibleVenueIdsAsync(string userSub, bool isSystemAdmin, bool isContentManager, CancellationToken cancellationToken = default)
    {
        try
        {
            // System admin and content manager have access to ALL venues
            if (isSystemAdmin || isContentManager)
            {
                var allVenues = await _venueRepository.GetAllAsync(cancellationToken);
                return allVenues.Select(v => v.Id);
            }

            // Regular users only get venues they have specific permissions for
            return await GetUserVenueIdsAsync(userSub, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accessible venue IDs for user {UserSub}", userSub);
            return Enumerable.Empty<long>();
        }
    }

    public async Task<IEnumerable<UserVenuePermissionEntity>> GetVenuePermissionsAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _permissionRepository.GetVenuePermissionsAsync(venueId, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for venue {VenueId}", venueId);
            return Enumerable.Empty<UserVenuePermissionEntity>();
        }
    }

    public async Task<IEnumerable<VenueInvitationEntity>> GetVenueInvitationsAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _invitationRepository.GetVenueInvitationsAsync(venueId, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitations for venue {VenueId}", venueId);
            return Enumerable.Empty<VenueInvitationEntity>();
        }
    }

    public async Task<UserVenuePermissionEntity?> GetUserVenuePermissionAsync(string email, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null) return null;

            return await _permissionRepository.GetUserVenuePermissionAsync(user.Id, venueId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user venue permission for email {Email} and venue {VenueId}", email, venueId);
            return null;
        }
    }

    public async Task<VenueInvitationEntity?> GetPendingInvitationAsync(string email, long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _invitationRepository.GetPendingInvitationAsync(email, venueId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending invitation for email {Email} and venue {VenueId}", email, venueId);
            return null;
        }
    }

    public async Task<VenueInvitationEntity?> CreateInvitationAsync(CreateInvitationRequest request, string invitedByUserSub, string invitedByUserEmail, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating invitation: Email={Email}, VenueId={VenueId}, Permission={Permission}, InvitedBy={InvitedByUserSub}, InvitedByEmail={InvitedByUserEmail}", 
                request.Email, request.VenueId, request.Permission, invitedByUserSub, invitedByUserEmail);
            
            // Ensure the inviting user exists WITH their email
            var invitingUserId = await EnsureUserExistsAsync(invitedByUserSub, invitedByUserEmail, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Inviting user ID: {InvitingUserId}", invitingUserId);
            
            var invitation = await _invitationRepository.CreateInvitationAsync(
                request.Email,
                request.VenueId,
                request.Permission,
                invitingUserId,
                7, // 7 days expiration
                request.Notes,
                cancellationToken);
                
            _logger.LogInformation("Created invitation with ID: {InvitationId}", invitation?.Id);
            
            await _invitationRepository.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Invitation saved to database successfully");
            
            return invitation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invitation for email {Email} and venue {VenueId}", request.Email, request.VenueId);
            return null;
        }
    }

    public async Task<IEnumerable<VenueInvitationEntity>> GetUserPendingInvitationsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _invitationRepository.GetPendingInvitationsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending invitations for email {Email}", email);
            return Enumerable.Empty<VenueInvitationEntity>();
        }
    }

    public async Task<bool> AcceptInvitationAsync(long invitationId, string userSub, string userEmail, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the invitation
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
            if (invitation == null || !invitation.IsValid)
            {
                _logger.LogWarning("Invalid invitation {InvitationId} for user {UserSub}", invitationId, userSub);
                return false;
            }

            // Verify email matches the invitation (security check)
            if (!string.Equals(userEmail, invitation.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Email mismatch when accepting invitation {InvitationId}. Expected {ExpectedEmail}, got {ActualEmail}", 
                    invitationId, invitation.Email, userEmail);
                return false;
            }

            // Ensure user exists in our database with their email
            var userId = await EnsureUserExistsAsync(userSub, userEmail, cancellationToken: cancellationToken);

            // Check if user already has permission for this venue
            var existingPermission = await _permissionRepository.GetUserVenuePermissionAsync(userId, invitation.VenueId, cancellationToken);
            if (existingPermission != null)
            {
                _logger.LogWarning("User {UserSub} already has permission for venue {VenueId}", userSub, invitation.VenueId);
                // Mark invitation as accepted anyway to clean up
                await _invitationRepository.MarkAsAcceptedAsync(invitationId, userId, _clock.GetCurrentInstant(), cancellationToken);
                return false;
            }

            // Create the permission
            var permissionResult = await _permissionRepository.GrantVenuePermissionAsync(
                userId,
                invitation.VenueId,
                invitation.Permission,
                invitation.InvitedByUserId,
                $"Granted via invitation {invitationId}",
                cancellationToken);
            
            if (permissionResult == null)
            {
                _logger.LogError("Failed to create permission for user {UserSub} and venue {VenueId}", userSub, invitation.VenueId);
                return false;
            }

            // Mark invitation as accepted
            await _invitationRepository.MarkAsAcceptedAsync(invitationId, userId, _clock.GetCurrentInstant(), cancellationToken);

            // Save all changes to database
            await _invitationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserSub} accepted invitation {InvitationId} for venue {VenueId} with permission {Permission}", 
                userSub, invitationId, invitation.VenueId, invitation.Permission);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting invitation {InvitationId} for user {UserSub}", invitationId, userSub);
            return false;
        }
    }

    public async Task<bool> DeclineInvitationAsync(long invitationId, string userSub, string userEmail, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the invitation
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
            if (invitation == null)
            {
                _logger.LogWarning("Invitation {InvitationId} not found for user {UserSub}", invitationId, userSub);
                return false;
            }

            // Verify email matches the invitation (security check)
            if (!string.Equals(userEmail, invitation.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Email mismatch when declining invitation {InvitationId}. Expected {ExpectedEmail}, got {ActualEmail}", 
                    invitationId, invitation.Email, userEmail);
                return false;
            }

            // Ensure user exists in our database with their email (they might be new)
            await EnsureUserExistsAsync(userSub, userEmail, cancellationToken: cancellationToken);

            // Mark invitation as declined
            await _invitationRepository.MarkAsDeclinedAsync(invitationId, cancellationToken);

            // Save changes to database
            await _invitationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserSub} declined invitation {InvitationId} for venue {VenueId}", 
                userSub, invitationId, invitation.VenueId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error declining invitation {InvitationId} for user {UserSub}", invitationId, userSub);
            return false;
        }
    }

    public async Task<VenueInvitationEntity?> GetInvitationByIdAsync(long invitationId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _invitationRepository.GetInvitationByIdAsync(invitationId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitation {InvitationId}", invitationId);
            return null;
        }
    }

    public async Task<bool> RevokeInvitationAsync(long invitationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _invitationRepository.RevokeInvitationAsync(invitationId, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking invitation {InvitationId}", invitationId);
            return false;
        }
    }

    public async Task<UserEntity?> GetUserBySubAsync(string userSub, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userRepository.GetBySubAsync(userSub, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by sub {UserSub}", userSub);
            return null;
        }
    }

    public async Task<UserVenuePermissionEntity?> UpdateUserVenuePermissionAsync(long permissionId, string permission, string updatedByUserSub, string? notes = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the permission to update
            var existingPermission = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (existingPermission == null || !existingPermission.IsActive)
            {
                _logger.LogWarning("Permission {PermissionId} not found or inactive", permissionId);
                return null;
            }

            // Update the permission
            existingPermission.Name = permission;
            existingPermission.Notes = notes;

            await _permissionRepository.UpdateAsync(existingPermission, cancellationToken);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission {PermissionId} updated to {Permission} by user {UserSub}", 
                permissionId, permission, updatedByUserSub);

            // Fetch the updated permission with includes
            var permissionsWithIncludes = await _permissionRepository.GetVenuePermissionsAsync(existingPermission.VenueId, cancellationToken: cancellationToken);
            return permissionsWithIncludes.FirstOrDefault(p => p.Id == permissionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission {PermissionId}", permissionId);
            return null;
        }
    }

    public async Task<bool> RevokeUserVenuePermissionAsync(long permissionId, string revokedByUserSub, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the permission to revoke
            var permission = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (permission == null || !permission.IsActive)
            {
                _logger.LogWarning("Permission {PermissionId} not found or already inactive", permissionId);
                return false;
            }

            // Mark as inactive (soft delete)
            permission.IsActive = false;

            await _permissionRepository.UpdateAsync(permission, cancellationToken);
            await _permissionRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permission {PermissionId} revoked by user {UserSub}", 
                permissionId, revokedByUserSub);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission {PermissionId}", permissionId);
            return false;
        }
    }
}
