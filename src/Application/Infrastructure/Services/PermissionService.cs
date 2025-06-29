using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Models.Auth;
using Application.Domain.Entities;
using Application.Infrastructure.Authorization.Permissions;
using Microsoft.Extensions.Logging;
using NodaTime;

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
                Permission = p.Name,
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
}
