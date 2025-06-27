using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for business hours operations
/// </summary>
public interface IBusinessHoursRepository : IRepository<BusinessHoursEntity, long>
{
    Task<IEnumerable<BusinessHoursEntity>> GetBusinessHoursByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default);
        
    Task<BusinessHoursEntity?> GetBusinessHoursForVenueAndDayAsync(
        long venueId, 
        byte dayOfWeekId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<BusinessHoursEntity>> GetBusinessHoursWithVenueDetailsAsync(
        CancellationToken cancellationToken = default);
}
