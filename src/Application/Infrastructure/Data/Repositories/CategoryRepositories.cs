using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for venue category operations
/// </summary>
public class VenueCategoryRepository : BaseRepository<VenueCategoryEntity, int>, IVenueCategoryRepository
{
    public VenueCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VenueCategoryEntity>> GetCategoriesWithVenuesAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(vc => vc.Venues.Where(v => v.IsActive))
            .OrderBy(vc => vc.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<VenueCategoryEntity?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .FirstOrDefaultAsync(vc => vc.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<VenueCategoryEntity>> GetCategoriesOrderedBySortOrderAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .OrderBy(vc => vc.SortOrder)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repository implementation for special category operations
/// </summary>
public class SpecialCategoryRepository : BaseRepository<SpecialCategoryEntity, int>, ISpecialCategoryRepository
{
    public SpecialCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SpecialCategoryEntity>> GetCategoriesWithSpecialsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(sc => sc.Specials.Where(s => s.IsActive))
            .OrderBy(sc => sc.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<SpecialCategoryEntity?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .FirstOrDefaultAsync(sc => sc.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<SpecialCategoryEntity>> GetCategoriesOrderedBySortOrderAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .OrderBy(sc => sc.SortOrder)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repository implementation for business hours operations
/// </summary>
public class BusinessHoursRepository : BaseRepository<BusinessHoursEntity, long>, IBusinessHoursRepository
{
    public BusinessHoursRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BusinessHoursEntity>> GetBusinessHoursByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Where(bh => bh.VenueId == venueId)
            .Include(bh => bh.DayOfWeek)
            .OrderBy(bh => bh.DayOfWeek.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessHoursEntity?> GetBusinessHoursForVenueAndDayAsync(
        long venueId, 
        byte dayOfWeekId, 
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(bh => bh.DayOfWeek)
            .FirstOrDefaultAsync(bh => bh.VenueId == venueId && bh.DayOfWeekId == dayOfWeekId, cancellationToken);
    }

    public async Task<IEnumerable<BusinessHoursEntity>> GetBusinessHoursWithVenueDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(bh => bh.Venue)
                .ThenInclude(v => v.Category)
            .Include(bh => bh.DayOfWeek)
            .OrderBy(bh => bh.Venue.Name)
            .ThenBy(bh => bh.DayOfWeek.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
