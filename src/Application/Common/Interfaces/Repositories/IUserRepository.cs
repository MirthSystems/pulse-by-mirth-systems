using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for user operations
/// </summary>
public interface IUserRepository : IRepository<UserEntity, long>
{
    /// <summary>
    /// Gets a user by their Auth0 subject identifier (sub claim)
    /// </summary>
    Task<UserEntity?> GetBySubAsync(
        string sub,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    Task<UserEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a user based on Auth0 information
    /// </summary>
    Task<UserEntity> UpsertUserAsync(
        string sub,
        string email,
        string? name = null,
        string? pictureUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the last login timestamp for a user
    /// </summary>
    Task UpdateLastLoginAsync(
        long userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active users with venue permissions
    /// </summary>
    Task<IEnumerable<UserEntity>> GetUsersWithVenuePermissionsAsync(
        CancellationToken cancellationToken = default);
}
