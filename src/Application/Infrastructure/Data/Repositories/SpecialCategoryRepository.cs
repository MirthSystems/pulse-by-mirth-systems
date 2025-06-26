using Application.Common.Interfaces.Repositories;
using Application.Domain.Entities;
using Application.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Data.Repositories;

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
