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
