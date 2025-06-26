using Application.Common.Models.Search;
using Application.Common.Models.Special;

namespace Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for special operations
/// </summary>
public interface ISpecialService
{
    // CRUD operations
    Task<ApiResponse<Special?>> GetSpecialByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetAllSpecialsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<Special>> CreateSpecialAsync(CreateSpecial createSpecial, CancellationToken cancellationToken = default);
    Task<ApiResponse<Special?>> UpdateSpecialAsync(long id, UpdateSpecial updateSpecial, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteSpecialAsync(long id, CancellationToken cancellationToken = default);

    // Specialized queries
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsByVenueAsync(long venueId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsByVenueAsync(long venueId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    // Time-based queries
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetRecurringSpecialsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsActiveNowAsync(CancellationToken cancellationToken = default);

    // Geospatial queries
    Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters = 5000, 
        CancellationToken cancellationToken = default);

    // Search operations
    Task<ApiResponse<PagedResponse<SpecialSummary>>> SearchSpecialsAsync(
        SpecialSearch search, 
        CancellationToken cancellationToken = default);

    // Enhanced search operations - returns venues with categorized specials
    Task<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>> SearchVenuesWithSpecialsAsync(
        EnhancedSpecialSearch searchRequest, 
        CancellationToken cancellationToken = default);

    // Category operations
    Task<ApiResponse<IEnumerable<SpecialCategory>>> GetSpecialCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<SpecialCategory?>> GetSpecialCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
}
