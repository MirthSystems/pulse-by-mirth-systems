using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Data.Repositories;

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
