using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for user operations
/// </summary>
public class UserRepository : BaseRepository<UserEntity, long>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserEntity?> GetBySubAsync(
        string sub,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .FirstOrDefaultAsync(u => u.Sub == sub, cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await Query
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<UserEntity> UpsertUserAsync(
        string sub,
        string email,
        string? name = null,
        string? pictureUrl = null,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await GetBySubAsync(sub, cancellationToken);
        var now = SystemClock.Instance.GetCurrentInstant();

        if (existingUser != null)
        {
            // Update existing user
            existingUser.Email = email;
            existingUser.UpdatedAt = now;
            existingUser.LastLoginAt = now;

            await UpdateAsync(existingUser, cancellationToken);
            return existingUser;
        }

        var newUser = new UserEntity
        {
            Sub = sub,
            Email = email,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            LastLoginAt = now
        };

        return await AddAsync(newUser, cancellationToken);
    }

    public async Task UpdateLastLoginAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.LastLoginAt = SystemClock.Instance.GetCurrentInstant();
            user.UpdatedAt = SystemClock.Instance.GetCurrentInstant();
            await UpdateAsync(user, cancellationToken);
        }
    }

    public async Task<IEnumerable<UserEntity>> GetUsersWithVenuePermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(u => u.VenuePermissions.Where(uvp => uvp.IsActive))
            .ThenInclude(uvp => uvp.Venue)
            .Where(u => u.IsActive && u.VenuePermissions.Any(uvp => uvp.IsActive))
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);
    }
}
