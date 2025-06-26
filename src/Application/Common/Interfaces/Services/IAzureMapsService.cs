using Application.Common.Models.Location;

namespace Application.Common.Interfaces.Services;

public interface IAzureMapsService
{
    /// <summary>
    /// Geocodes an address to coordinates using Azure Maps
    /// </summary>
    Task<GeocodeResult?> GeocodeAddressAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverse geocodes coordinates to address information using Azure Maps
    /// </summary>
    Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for points of interest near a location using Azure Maps
    /// </summary>
    Task<IEnumerable<PointOfInterest>> SearchNearbyAsync(double latitude, double longitude, string? category = null, int radiusInMeters = 5000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets timezone information for a location using Azure Maps
    /// </summary>
    Task<TimeZoneInfo?> GetTimeZoneAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}
