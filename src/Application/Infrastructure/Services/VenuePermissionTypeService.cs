using Application.Common.Interfaces.Services;
using Application.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Infrastructure.Services;

/// <summary>
/// Service for managing venue permission types and their validation
/// </summary>
public class VenuePermissionTypeService : IVenuePermissionTypeService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<VenuePermissionTypeService> _logger;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(1);
    private const string CACHE_KEY = "venue_permission_types";

    public VenuePermissionTypeService(
        IMemoryCache cache,
        ILogger<VenuePermissionTypeService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<string>> GetValidPermissionTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(CACHE_KEY, entry =>
        {
            entry.SetAbsoluteExpiration(_cacheExpiry);
            
            _logger.LogDebug("Loading valid permission types into cache");
            
            // For now, return the static list, but this could be expanded to load from database
            // if you need dynamic permission types in the future
            return Task.FromResult(GetStaticPermissionTypes());
        }) ?? GetStaticPermissionTypes();
    }

    public async Task<bool> IsValidPermissionTypeAsync(string permissionType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionType))
            return false;

        var validTypes = await GetValidPermissionTypesAsync(cancellationToken);
        return validTypes.Contains(permissionType, StringComparer.OrdinalIgnoreCase);
    }

    public Task<string> GetPermissionDisplayNameAsync(string permissionType, CancellationToken cancellationToken = default)
    {
        var displayName = permissionType switch
        {
            "venue:owner" => "Owner",
            "venue:manager" => "Manager",
            "venue:staff" => "Staff",
            _ => permissionType
        };
        
        return Task.FromResult(displayName);
    }

    public Task<string[]> GetPermissionHierarchyAsync(string permissionType, CancellationToken cancellationToken = default)
    {
        var hierarchy = permissionType switch
        {
            "venue:owner" => new[] { "venue:owner", "venue:manager", "venue:staff" },
            "venue:manager" => new[] { "venue:manager", "venue:staff" },
            "venue:staff" => new[] { "venue:staff" },
            _ => Array.Empty<string>()
        };
        
        return Task.FromResult(hierarchy);
    }

    public void InvalidateCache()
    {
        _cache.Remove(CACHE_KEY);
        _logger.LogDebug("Permission types cache invalidated");
    }

    private static IEnumerable<string> GetStaticPermissionTypes()
    {
        return new[]
        {
            "venue:owner",
            "venue:manager", 
            "venue:staff"
        };
    }
}
