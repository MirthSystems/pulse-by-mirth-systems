using Application.Common.Interfaces.Services;
using Application.Common.Models.Location;
using Application.Common.Models.AzureMaps;

using Azure;
using Azure.Core.GeoJson;
using Azure.Maps.Geolocation;
using Azure.Maps.Search;
using Azure.Maps.TimeZones;
using System.Text.Json;

namespace Application.Infrastructure.Services;

public class AzureMapsService : IAzureMapsService
{
    private readonly MapsSearchClient _searchClient;
    private readonly MapsGeolocationClient _geolocationClient;    
    private readonly MapsTimeZoneClient _timeZoneClient;
    private readonly HttpClient _httpClient;
    private readonly string _subscriptionKey;

    public AzureMapsService(AzureKeyCredential azureMapsKeyCredential)
    {
        _searchClient = new MapsSearchClient(azureMapsKeyCredential);
        _geolocationClient = new MapsGeolocationClient(azureMapsKeyCredential);       
        _timeZoneClient = new MapsTimeZoneClient(azureMapsKeyCredential);
        
        // Store the key for REST API calls
        _subscriptionKey = azureMapsKeyCredential.Key;
        _httpClient = new HttpClient();
    }

    public async Task<GeocodeResult?> GeocodeAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _searchClient.GetGeocodingAsync(address, cancellationToken: cancellationToken);
            
            if (response?.Value?.Features?.Count > 0)
            {
                var feature = response.Value.Features[0];
                var addressProps = feature.Properties?.Address;
                
                return new GeocodeResult
                {
                    FormattedAddress = addressProps?.FormattedAddress ?? address,
                    Latitude = feature.Geometry.Coordinates[1], // GeoJSON uses [longitude, latitude]
                    Longitude = feature.Geometry.Coordinates[0],
                    Street = addressProps?.AddressLine ?? string.Empty,
                    City = addressProps?.Locality ?? string.Empty,
                    Region = addressProps?.AdminDistricts?.FirstOrDefault()?.Name ?? string.Empty,
                    PostalCode = addressProps?.PostalCode ?? string.Empty,
                    Country = addressProps?.CountryRegion?.Name ?? string.Empty,
                    Confidence = 0.95 // High confidence placeholder since Azure Maps doesn't provide this
                };
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReverseGeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var coordinates = new GeoPosition(longitude, latitude); // GeoPosition expects (longitude, latitude)
            var response = await _searchClient.GetReverseGeocodingAsync(coordinates, cancellationToken: cancellationToken);
            
            if (response?.Value?.Features?.Count > 0)
            {
                var feature = response.Value.Features[0];
                var addressProps = feature.Properties?.Address;
                
                return new ReverseGeocodeResult
                {
                    FormattedAddress = addressProps?.FormattedAddress ?? string.Empty,
                    Street = addressProps?.AddressLine ?? string.Empty,
                    City = addressProps?.Locality ?? string.Empty,
                    Region = addressProps?.AdminDistricts?.FirstOrDefault()?.Name ?? string.Empty,
                    PostalCode = addressProps?.PostalCode ?? string.Empty,
                    Country = addressProps?.CountryRegion?.Name ?? string.Empty,
                    Neighborhood = addressProps?.Neighborhood ?? string.Empty
                };
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<PointOfInterest>> SearchNearbyAsync(double latitude, double longitude, string? category = null, int radiusInMeters = 5000, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use Azure Maps POI Search REST API
            var searchTerm = category ?? "restaurant"; // Default to restaurants if no category specified
            
            // Build the REST API URL
            var baseUrl = "https://atlas.microsoft.com/search/poi/json";
            var queryParams = new List<string>
            {
                "api-version=1.0",
                $"subscription-key={_subscriptionKey}",
                $"query={Uri.EscapeDataString(searchTerm)}",
                $"lat={latitude:F6}",
                $"lon={longitude:F6}",
                $"radius={radiusInMeters}",
                "limit=20" // Limit to 20 results
            };
            
            var requestUrl = $"{baseUrl}?{string.Join("&", queryParams)}";
            
            // Make the HTTP request
            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var searchResult = JsonSerializer.Deserialize<AzureMapsPointOfInterestSearchResponse>(jsonContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            if (searchResult?.Results?.Any() == true)
            {
                return searchResult.Results.Select(result => new PointOfInterest
                {
                    Name = result.Poi?.Name ?? "Unknown",
                    Category = result.Poi?.Classifications?.FirstOrDefault()?.Names?.FirstOrDefault()?.Name ?? "Unknown",
                    Address = result.Address?.FreeformAddress ?? string.Empty,
                    Latitude = result.Position?.Lat ?? 0,
                    Longitude = result.Position?.Lon ?? 0,
                    Phone = result.Poi?.Phone ?? string.Empty,
                    Website = result.Poi?.Url ?? string.Empty,
                    DistanceInMeters = result.Dist,
                    Score = result.Score
                });
            }
            
            return Enumerable.Empty<PointOfInterest>();
        }
        catch
        {
            return Enumerable.Empty<PointOfInterest>();
        }
    }

    public async Task<TimeZoneInfo?> GetTimeZoneAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var coordinates = new GeoPosition(longitude, latitude); // GeoPosition expects (longitude, latitude)
            var options = new GetTimeZoneOptions
            {
                AdditionalTimeZoneReturnInformation = AdditionalTimeZoneReturnInformation.All
            };
            var response = await _timeZoneClient.GetTimeZoneByCoordinatesAsync(coordinates, options, cancellationToken);
            
            if (response?.Value?.TimeZones?.Count > 0)
            {
                var timeZoneData = response.Value.TimeZones[0];
                
                // Try to convert IANA time zone ID to .NET TimeZoneInfo
                try 
                {
                    // Try to find by IANA ID first
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneData.Id);
                }
                catch
                {
                    // If IANA ID doesn't work, try some common conversions or return UTC
                    return TimeZoneInfo.Utc;
                }
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<GeocodeResult>> SearchAddressAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
                return Enumerable.Empty<GeocodeResult>();

            var response = await _searchClient.GetGeocodingAsync(query, cancellationToken: cancellationToken);
            
            if (response?.Value?.Features?.Any() == true)
            {
                return response.Value.Features
                    .Take(10) // Limit to top 10 suggestions
                    .Select(feature =>
                    {
                        var addressProps = feature.Properties?.Address;
                        return new GeocodeResult
                        {
                            FormattedAddress = addressProps?.FormattedAddress ?? query,
                            Latitude = feature.Geometry.Coordinates[1], // GeoJSON uses [longitude, latitude]
                            Longitude = feature.Geometry.Coordinates[0],
                            Street = addressProps?.AddressLine ?? string.Empty,
                            City = addressProps?.Locality ?? string.Empty,
                            Region = addressProps?.AdminDistricts?.FirstOrDefault()?.Name ?? string.Empty,
                            PostalCode = addressProps?.PostalCode ?? string.Empty,
                            Country = addressProps?.CountryRegion?.Name ?? string.Empty,
                            Confidence = 0.95 // High confidence placeholder since Azure Maps doesn't provide this
                        };
                    });
            }
            
            return Enumerable.Empty<GeocodeResult>();
        }
        catch
        {
            return Enumerable.Empty<GeocodeResult>();
        }
    }
}
