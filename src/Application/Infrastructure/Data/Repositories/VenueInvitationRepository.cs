using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for venue invitation operations
/// </summary>
public class VenueInvitationRepository : BaseRepository<VenueInvitationEntity, long>, IVenueInvitationRepository
{
    public VenueInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VenueInvitationEntity>> GetPendingInvitationsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(vi => vi.Venue)
            .Include(vi => vi.InvitedByUser)
            .Where(vi => vi.Email.ToLower() == email.ToLower() && 
                        vi.IsActive && 
                        !vi.AcceptedAt.HasValue &&
                        vi.ExpiresAt > SystemClock.Instance.GetCurrentInstant())
            .OrderBy(vi => vi.InvitedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueInvitationEntity>> GetVenueInvitationsAsync(
        long venueId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query
            .Include(vi => vi.InvitedByUser)
            .Include(vi => vi.AcceptedByUser)
            .Where(vi => vi.VenueId == venueId);

        if (activeOnly)
        {
            query = query.Where(vi => vi.IsActive);
        }

        return await query
            .OrderByDescending(vi => vi.InvitedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueInvitationEntity>> GetInvitationsSentByUserAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(vi => vi.Venue)
            .Include(vi => vi.AcceptedByUser)
            .Where(vi => vi.InvitedByUserId == userId)
            .OrderByDescending(vi => vi.InvitedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<VenueInvitationEntity?> GetInvitationByIdAsync(
        long invitationId,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(vi => vi.Venue)
            .Include(vi => vi.InvitedByUser)
            .Include(vi => vi.AcceptedByUser)
            .FirstOrDefaultAsync(vi => vi.Id == invitationId, cancellationToken);
    }

    public async Task<VenueInvitationEntity> CreateInvitationAsync(
        string email,
        long venueId,
        string permission,
        long invitedByUserId,
        int expirationDays = 7,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        var expiresAt = now.Plus(Duration.FromDays(expirationDays));

        var invitation = new VenueInvitationEntity
        {
            Email = email,
            VenueId = venueId,
            Permission = permission,
            InvitedByUserId = invitedByUserId,
            InvitedAt = now,
            ExpiresAt = expiresAt,
            IsActive = true,
            Notes = notes
        };

        return await AddAsync(invitation, cancellationToken);
    }

    public async Task<UserVenuePermissionEntity?> AcceptInvitationAsync(
        long invitationId,
        long acceptedByUserId,
        CancellationToken cancellationToken = default)
    {
        var invitation = await GetInvitationByIdAsync(invitationId, cancellationToken);
        if (invitation == null || !invitation.IsValid)
        {
            return null;
        }

        var now = SystemClock.Instance.GetCurrentInstant();

        // Mark invitation as accepted
        invitation.AcceptedAt = now;
        invitation.AcceptedByUserId = acceptedByUserId;
        await UpdateAsync(invitation, cancellationToken);

        // Create the permission record
        var permission = new UserVenuePermissionEntity
        {
            UserId = acceptedByUserId,
            VenueId = invitation.VenueId,
            Name = invitation.Permission,
            GrantedByUserId = invitation.InvitedByUserId,
            GrantedAt = now,
            IsActive = true,
            Notes = $"Accepted invitation on {now.ToDateTimeUtc():yyyy-MM-dd}"
        };

        var userPermissionRepo = new UserVenuePermissionRepository(Context);
        return await userPermissionRepo.AddAsync(permission, cancellationToken);
    }

    public async Task<bool> RevokeInvitationAsync(
        long invitationId,
        CancellationToken cancellationToken = default)
    {
        var invitation = await GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            return false;
        }

        invitation.IsActive = false;
        await UpdateAsync(invitation, cancellationToken);
        return true;
    }

    public async Task<int> CleanupExpiredInvitationsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        var expiredInvitations = await Query
            .Where(vi => vi.IsActive && vi.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        foreach (var invitation in expiredInvitations)
        {
            invitation.IsActive = false;
        }

        if (expiredInvitations.Any())
        {
            await SaveChangesAsync(cancellationToken);
        }

        return expiredInvitations.Count;
    }

    public async Task<bool> HasPendingInvitationAsync(
        string email,
        long venueId,
        CancellationToken cancellationToken = default)
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        return await Query
            .AnyAsync(vi => vi.Email.ToLower() == email.ToLower() &&
                           vi.VenueId == venueId &&
                           vi.IsActive &&
                           !vi.AcceptedAt.HasValue &&
                           vi.ExpiresAt > now, 
                     cancellationToken);
    }
}
