using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for venue category operations
/// </summary>
public interface IVenueCategoryRepository : IRepository<VenueCategoryEntity, int>
{
    Task<IEnumerable<VenueCategoryEntity>> GetCategoriesWithVenuesAsync(
        CancellationToken cancellationToken = default);
        
    Task<VenueCategoryEntity?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueCategoryEntity>> GetCategoriesOrderedBySortOrderAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for special category operations
/// </summary>
public interface ISpecialCategoryRepository : IRepository<SpecialCategoryEntity, int>
{
    Task<IEnumerable<SpecialCategoryEntity>> GetCategoriesWithSpecialsAsync(
        CancellationToken cancellationToken = default);
        
    Task<SpecialCategoryEntity?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialCategoryEntity>> GetCategoriesOrderedBySortOrderAsync(
        CancellationToken cancellationToken = default);
}

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
