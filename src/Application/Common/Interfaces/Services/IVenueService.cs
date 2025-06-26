using Application.Common.Models.Location;
using Application.Common.Models.Search;
using Application.Common.Models.Venue;

namespace Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for venue operations
/// </summary>
public interface IVenueService
{
    // CRUD operations
    Task<ApiResponse<Venue?>> GetVenueByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<VenueSummary>>> GetAllVenuesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<Venue>> CreateVenueAsync(CreateVenue createVenue, CancellationToken cancellationToken = default);
    Task<ApiResponse<Venue?>> UpdateVenueAsync(long id, UpdateVenue updateVenue, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteVenueAsync(long id, CancellationToken cancellationToken = default);

    // Specialized queries
    Task<ApiResponse<IEnumerable<VenueSummary>>> GetActiveVenuesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesWithActiveSpecialsAsync(CancellationToken cancellationToken = default);
    
    // Geospatial queries
    Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters = 5000, 
        CancellationToken cancellationToken = default);

    // Search operations
    Task<ApiResponse<PagedResponse<VenueSummary>>> SearchVenuesAsync(
        VenueSearch search, 
        CancellationToken cancellationToken = default);

    // Category operations
    Task<ApiResponse<IEnumerable<VenueCategory>>> GetVenueCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<VenueCategory?>> GetVenueCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

    // Azure Maps enhanced location methods
    Task<ApiResponse<GeocodeResult?>> GeocodeVenueAddressAsync(long venueId, CancellationToken cancellationToken = default);
    Task<ApiResponse<ReverseGeocodeResult?>> GetVenueLocationDetailsAsync(long venueId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<PointOfInterest>>> GetNearbyPointsOfInterestAsync(long venueId, string? category = null, int radiusInMeters = 1000, CancellationToken cancellationToken = default);
    Task<ApiResponse<TimeZoneInfo?>> GetVenueTimeZoneAsync(long venueId, CancellationToken cancellationToken = default);
    Task<ApiResponse<EnhancedVenueSearchResult>> SearchVenuesWithPOIDataAsync(VenueSearch search, CancellationToken cancellationToken = default);
}
