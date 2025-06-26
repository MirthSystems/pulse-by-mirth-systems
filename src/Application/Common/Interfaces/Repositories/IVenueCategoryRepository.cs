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
