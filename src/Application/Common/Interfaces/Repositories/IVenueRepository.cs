using Application.Domain.Entities;
using NetTopologySuite.Geometries;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for venue-specific operations
/// </summary>
public interface IVenueRepository : IRepository<VenueEntity, long>
{
    // Geospatial queries
    Task<IEnumerable<VenueEntity>> GetVenuesNearLocationAsync(
        Point location, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueEntity>> GetVenuesInRadiusAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default);

    // Category-based queries
    Task<IEnumerable<VenueEntity>> GetVenuesByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueEntity>> GetActiveVenuesAsync(
        CancellationToken cancellationToken = default);

    // Complex queries with includes
    Task<VenueEntity?> GetVenueWithDetailsAsync(
        long venueId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueEntity>> GetVenuesWithActiveSpecialsAsync(
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueEntity>> GetVenuesWithBusinessHoursAsync(
        CancellationToken cancellationToken = default);

    // Search operations
    Task<IEnumerable<VenueEntity>> SearchVenuesByNameAsync(
        string searchTerm, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<VenueEntity>> SearchVenuesAsync(
        string? searchTerm = null,
        int? categoryId = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInMeters = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);
}
