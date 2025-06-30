using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for venue invitation operations
/// </summary>
public interface IVenueInvitationRepository : IRepository<VenueInvitationEntity, long>
{
    /// <summary>
    /// Gets all pending invitations for an email address
    /// </summary>
    Task<IEnumerable<VenueInvitationEntity>> GetPendingInvitationsAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all invitations for a specific venue
    /// </summary>
    Task<IEnumerable<VenueInvitationEntity>> GetVenueInvitationsAsync(
        long venueId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all invitations sent by a specific user
    /// </summary>
    Task<IEnumerable<VenueInvitationEntity>> GetInvitationsSentByUserAsync(
        long userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific invitation by ID
    /// </summary>
    Task<VenueInvitationEntity?> GetInvitationByIdAsync(
        long invitationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new venue invitation
    /// </summary>
    Task<VenueInvitationEntity> CreateInvitationAsync(
        string email,
        long venueId,
        string permission,
        long invitedByUserId,
        int expirationDays = 7,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Accepts an invitation and creates the corresponding permission
    /// </summary>
    Task<UserVenuePermissionEntity?> AcceptInvitationAsync(
        long invitationId,
        long acceptedByUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an invitation by setting it inactive
    /// </summary>
    Task<bool> RevokeInvitationAsync(
        long invitationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired invitations
    /// </summary>
    Task<int> CleanupExpiredInvitationsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there's already a pending invitation for an email/venue combination
    /// </summary>
    Task<bool> HasPendingInvitationAsync(
        string email,
        long venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pending invitation for a specific email/venue combination
    /// </summary>
    Task<VenueInvitationEntity?> GetPendingInvitationAsync(
        string email,
        long venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an invitation as accepted
    /// </summary>
    Task<bool> MarkAsAcceptedAsync(
        long invitationId,
        long acceptedByUserId,
        NodaTime.Instant acceptedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an invitation as declined
    /// </summary>
    Task<bool> MarkAsDeclinedAsync(
        long invitationId,
        CancellationToken cancellationToken = default);
}
