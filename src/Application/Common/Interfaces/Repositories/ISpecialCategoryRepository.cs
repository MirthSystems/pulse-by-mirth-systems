using Application.Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

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
