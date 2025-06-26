using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for venue operations
/// </summary>
public class VenueRepository : BaseRepository<VenueEntity, long>, IVenueRepository
{
    public VenueRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VenueEntity>> GetVenuesNearLocationAsync(
        Point location, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(v => v.IsActive && v.Location != null && v.Location.IsWithinDistance(location, radiusInMeters))
            .Include(v => v.Category)
            .OrderBy(v => v.Location!.Distance(location))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> GetVenuesInRadiusAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default)
    {
        var location = new Point(longitude, latitude) { SRID = 4326 };
        return await GetVenuesNearLocationAsync(location, radiusInMeters, cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> GetVenuesByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(v => v.CategoryId == categoryId && v.IsActive)
            .Include(v => v.Category)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> GetActiveVenuesAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(v => v.IsActive)
            .Include(v => v.Category)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<VenueEntity?> GetVenueWithDetailsAsync(
        long venueId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(v => v.Category)
            .Include(v => v.BusinessHours)
                .ThenInclude(bh => bh.DayOfWeek)
            .Include(v => v.Specials.Where(s => s.IsActive))
                .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(v => v.Id == venueId, cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> GetVenuesWithActiveSpecialsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(v => v.IsActive && v.Specials.Any(s => s.IsActive))
            .Include(v => v.Category)
            .Include(v => v.Specials.Where(s => s.IsActive))
                .ThenInclude(s => s.Category)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> GetVenuesWithBusinessHoursAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(v => v.IsActive)
            .Include(v => v.Category)
            .Include(v => v.BusinessHours)
                .ThenInclude(bh => bh.DayOfWeek)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> SearchVenuesByNameAsync(
        string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm.ToLower().Trim();
        
        return await Query
            .Where(v => v.IsActive && 
                       (v.Name.ToLower().Contains(normalizedSearchTerm) ||
                        (v.Description != null && v.Description.ToLower().Contains(normalizedSearchTerm))))
            .Include(v => v.Category)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VenueEntity>> SearchVenuesAsync(
        string? searchTerm = null,
        int? categoryId = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInMeters = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query.AsQueryable();

        // Apply active filter
        if (activeOnly)
        {
            query = query.Where(v => v.IsActive);
        }

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.ToLower().Trim();
            query = query.Where(v => 
                v.Name.ToLower().Contains(normalizedSearchTerm) ||
                (v.Description != null && v.Description.ToLower().Contains(normalizedSearchTerm)));
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(v => v.CategoryId == categoryId.Value);
        }

        // Apply location filter
        if (latitude.HasValue && longitude.HasValue && radiusInMeters.HasValue)
        {
            var location = new Point(longitude.Value, latitude.Value) { SRID = 4326 };
            query = query.Where(v => v.Location != null && v.Location.IsWithinDistance(location, radiusInMeters.Value));
            query = query.OrderBy(v => v.Location!.Distance(location));
        }
        else
        {
            query = query.OrderBy(v => v.Name);
        }

        return await query
            .Include(v => v.Category)
            .ToListAsync(cancellationToken);
    }
}
