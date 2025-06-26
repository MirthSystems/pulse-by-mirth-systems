using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for special operations
/// </summary>
public class SpecialRepository : BaseRepository<SpecialEntity, long>, ISpecialRepository
{
    public SpecialRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SpecialEntity>> GetActiveSpecialsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.IsActive)
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetSpecialsActiveAtTimeAsync(
        LocalDateTime dateTime, 
        CancellationToken cancellationToken = default)
    {
        var date = dateTime.Date;
        var time = dateTime.TimeOfDay;

        return await Query
            .Where(s => s.IsActive &&
                       s.StartDate <= date &&
                       (s.EndDate == null || s.EndDate >= date) &&
                       s.StartTime <= time &&
                       (s.EndTime == null || s.EndTime >= time))
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetRecurringSpecialsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.IsActive && s.IsRecurring)
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetSpecialsByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.VenueId == venueId)
            .Include(s => s.Category)
            .OrderBy(s => s.StartDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetActiveSpecialsByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.VenueId == venueId && s.IsActive)
            .Include(s => s.Category)
            .OrderBy(s => s.StartDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetSpecialsByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.SpecialCategoryId == categoryId)
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetActiveSpecialsByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.SpecialCategoryId == categoryId && s.IsActive)
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<SpecialEntity?> GetSpecialWithDetailsAsync(
        long specialId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == specialId, cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetSpecialsWithVenueDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(s => s.IsActive)
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> GetActiveSpecialsNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default)
    {
        var location = new Point(longitude, latitude) { SRID = 4326 };

        return await Query
            .Where(s => s.IsActive && 
                       s.Venue.IsActive && 
                       s.Venue.Location != null && 
                       s.Venue.Location.IsWithinDistance(location, radiusInMeters))
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Location!.Distance(location))
            .ThenBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpecialEntity>> SearchSpecialsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        long? venueId = null,
        LocalDate? startDate = null,
        LocalDate? endDate = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = Query.AsQueryable();

        // Apply active filter
        if (activeOnly)
        {
            query = query.Where(s => s.IsActive && s.Venue.IsActive);
        }

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.ToLower().Trim();
            query = query.Where(s => 
                s.Title.ToLower().Contains(normalizedSearchTerm) ||
                (s.Description != null && s.Description.ToLower().Contains(normalizedSearchTerm)));
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(s => s.SpecialCategoryId == categoryId.Value);
        }

        // Apply venue filter
        if (venueId.HasValue)
        {
            query = query.Where(s => s.VenueId == venueId.Value);
        }

        // Apply date range filters
        if (startDate.HasValue)
        {
            query = query.Where(s => s.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.EndDate == null || s.EndDate <= endDate.Value);
        }

        return await query
            .Include(s => s.Venue)
                .ThenInclude(v => v.Category)
            .Include(s => s.Category)
            .OrderBy(s => s.Venue.Name)
            .ThenBy(s => s.StartDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }
}
