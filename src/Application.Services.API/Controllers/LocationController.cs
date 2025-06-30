using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Location;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for location-based operations including geocoding and address search
/// </summary>
[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    private readonly IAzureMapsService _azureMapsService;
    private readonly ILogger<LocationController> _logger;

    public LocationController(IAzureMapsService azureMapsService, ILogger<LocationController> logger)
    {
        _azureMapsService = azureMapsService ?? throw new ArgumentNullException(nameof(azureMapsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search for address suggestions using Azure Maps
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<GeocodeResult>>>> SearchAddresses(
        [FromQuery] string query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching addresses with query: {Query}", query);
        
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return BadRequest(ApiResponse<IEnumerable<GeocodeResult>>.ErrorResult("Query must be at least 3 characters long"));
        }

        try
        {
            var results = await _azureMapsService.SearchAddressAsync(query, cancellationToken);
            return Ok(ApiResponse<IEnumerable<GeocodeResult>>.SuccessResult(results));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching addresses with query: {Query}", query);
            return StatusCode(500, ApiResponse<IEnumerable<GeocodeResult>>.ErrorResult($"Error searching addresses: {ex.Message}"));
        }
    }

    /// <summary>
    /// Geocode a single address to coordinates
    /// </summary>
    [HttpPost("geocode")]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> GeocodeAddress(
        [FromBody] GeocodeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Geocoding address: {Address}", request.Address);
        
        if (string.IsNullOrWhiteSpace(request.Address))
        {
            return BadRequest(ApiResponse<GeocodeResult?>.ErrorResult("Address is required"));
        }

        try
        {
            var result = await _azureMapsService.GeocodeAddressAsync(request.Address, cancellationToken);
            return Ok(ApiResponse<GeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", request.Address);
            return StatusCode(500, ApiResponse<GeocodeResult?>.ErrorResult($"Error geocoding address: {ex.Message}"));
        }
    }

    /// <summary>
    /// Reverse geocode coordinates to address information
    /// </summary>
    [HttpPost("reverse-geocode")]
    public async Task<ActionResult<ApiResponse<ReverseGeocodeResult?>>> ReverseGeocode(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reverse geocoding coordinates: {Latitude}, {Longitude}", latitude, longitude);
        
        try
        {
            var result = await _azureMapsService.ReverseGeocodeAsync(latitude, longitude, cancellationToken);
            return Ok(ApiResponse<ReverseGeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverse geocoding coordinates: {Latitude}, {Longitude}", latitude, longitude);
            return StatusCode(500, ApiResponse<ReverseGeocodeResult?>.ErrorResult($"Error reverse geocoding: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get timezone information for coordinates
    /// </summary>
    [HttpGet("timezone")]
    public async Task<ActionResult<ApiResponse<TimeZoneInfo?>>> GetTimeZone(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting timezone for coordinates: {Latitude}, {Longitude}", latitude, longitude);
        
        try
        {
            var result = await _azureMapsService.GetTimeZoneAsync(latitude, longitude, cancellationToken);
            return Ok(ApiResponse<TimeZoneInfo?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timezone for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            return StatusCode(500, ApiResponse<TimeZoneInfo?>.ErrorResult($"Error getting timezone: {ex.Message}"));
        }
    }

    /// <summary>
    /// Validate and geocode a complete address from components
    /// </summary>
    [HttpPost("validate-address")]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> ValidateAddress(
        [FromBody] ValidateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating address: {StreetAddress}, {Locality}, {Region}, {PostalCode}", 
            request.StreetAddress, request.Locality, request.Region, request.PostalCode);
        
        try
        {
            // Build full address string
            var addressParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.StreetAddress)) addressParts.Add(request.StreetAddress.Trim());
            if (!string.IsNullOrWhiteSpace(request.SecondaryAddress)) addressParts.Add(request.SecondaryAddress.Trim());
            if (!string.IsNullOrWhiteSpace(request.Locality)) addressParts.Add(request.Locality.Trim());
            if (!string.IsNullOrWhiteSpace(request.Region)) addressParts.Add(request.Region.Trim());
            if (!string.IsNullOrWhiteSpace(request.PostalCode)) addressParts.Add(request.PostalCode.Trim());
            if (!string.IsNullOrWhiteSpace(request.Country)) addressParts.Add(request.Country.Trim());

            var fullAddress = string.Join(", ", addressParts);
            
            if (string.IsNullOrWhiteSpace(fullAddress))
            {
                return BadRequest(ApiResponse<GeocodeResult?>.ErrorResult("At least one address component is required"));
            }

            var result = await _azureMapsService.GeocodeAddressAsync(fullAddress, cancellationToken);
            
            if (result == null)
            {
                return Ok(ApiResponse<GeocodeResult?>.ErrorResult("Address could not be validated or geocoded"));
            }

            return Ok(ApiResponse<GeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating address: {StreetAddress}, {Locality}, {Region}, {PostalCode}", 
                request.StreetAddress, request.Locality, request.Region, request.PostalCode);
            return StatusCode(500, ApiResponse<GeocodeResult?>.ErrorResult($"Error validating address: {ex.Message}"));
        }
    }
}
