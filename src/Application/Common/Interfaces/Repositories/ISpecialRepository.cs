using Application.Domain.Entities;
using NodaTime;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for special-specific operations
/// </summary>
public interface ISpecialRepository : IRepository<SpecialEntity, long>
{
    // Time-based queries
    Task<IEnumerable<SpecialEntity>> GetActiveSpecialsAsync(
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialEntity>> GetSpecialsActiveAtTimeAsync(
        LocalDateTime dateTime, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialEntity>> GetRecurringSpecialsAsync(
        CancellationToken cancellationToken = default);

    // Venue-based queries
    Task<IEnumerable<SpecialEntity>> GetSpecialsByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialEntity>> GetActiveSpecialsByVenueAsync(
        long venueId, 
        CancellationToken cancellationToken = default);

    // Category-based queries
    Task<IEnumerable<SpecialEntity>> GetSpecialsByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialEntity>> GetActiveSpecialsByCategoryAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);

    // Complex queries with includes
    Task<SpecialEntity?> GetSpecialWithDetailsAsync(
        long specialId, 
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<SpecialEntity>> GetSpecialsWithVenueDetailsAsync(
        CancellationToken cancellationToken = default);

    // Geospatial queries for specials
    Task<IEnumerable<SpecialEntity>> GetActiveSpecialsNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters, 
        CancellationToken cancellationToken = default);

    // Search and filter operations
    Task<IEnumerable<SpecialEntity>> SearchSpecialsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        long? venueId = null,
        LocalDate? startDate = null,
        LocalDate? endDate = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);
}
