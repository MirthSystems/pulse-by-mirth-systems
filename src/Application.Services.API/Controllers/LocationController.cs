using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Location;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for location-based operations including geocoding and address search
/// </summary>
[ApiController]
public class LocationController : BaseApiController
{
    private readonly IAzureMapsService _azureMapsService;

    public LocationController(IAzureMapsService azureMapsService, ILogger<LocationController> logger)
        : base(logger)
    {
        _azureMapsService = azureMapsService ?? throw new ArgumentNullException(nameof(azureMapsService));
    }

    /// <summary>
    /// Search for address suggestions using Azure Maps
    /// </summary>
    [HttpGet(ApiRoutes.Location.Search)]
    public async Task<ActionResult<ApiResponse<IEnumerable<GeocodeResult>>>> SearchAddresses(
        [FromQuery] string query,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SearchAddresses), new { Query = query });
        
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            LogActionComplete(nameof(SearchAddresses), false, "Query too short");
            return BadRequest(ApiResponse<IEnumerable<GeocodeResult>>.ErrorResult("Query must be at least 3 characters long"));
        }

        try
        {
            var results = await _azureMapsService.SearchAddressAsync(query, cancellationToken);
            LogActionComplete(nameof(SearchAddresses), true);
            return Ok(ApiResponse<IEnumerable<GeocodeResult>>.SuccessResult(results));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SearchAddresses), new { Query = query });
            return InternalServerError<IEnumerable<GeocodeResult>>();
        }
    }

    /// <summary>
    /// Geocode a single address to coordinates
    /// </summary>
    [HttpPost(ApiRoutes.Location.Geocode)]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> GeocodeAddress(
        [FromBody] GeocodeRequest request,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GeocodeAddress), request);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        if (string.IsNullOrWhiteSpace(request.Address))
        {
            LogActionComplete(nameof(GeocodeAddress), false, "Address required");
            return BadRequest(ApiResponse<GeocodeResult?>.ErrorResult("Address is required"));
        }

        try
        {
            var result = await _azureMapsService.GeocodeAddressAsync(request.Address, cancellationToken);
            LogActionComplete(nameof(GeocodeAddress), true);
            return Ok(ApiResponse<GeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GeocodeAddress), request);
            return InternalServerError<GeocodeResult?>();
        }
    }

    /// <summary>
    /// Reverse geocode coordinates to address information
    /// </summary>
    [HttpPost(ApiRoutes.Location.ReverseGeocode)]
    public async Task<ActionResult<ApiResponse<ReverseGeocodeResult?>>> ReverseGeocode(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(ReverseGeocode), new { Latitude = latitude, Longitude = longitude });
        
        try
        {
            var result = await _azureMapsService.ReverseGeocodeAsync(latitude, longitude, cancellationToken);
            LogActionComplete(nameof(ReverseGeocode), true);
            return Ok(ApiResponse<ReverseGeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(ReverseGeocode), new { Latitude = latitude, Longitude = longitude });
            return InternalServerError<ReverseGeocodeResult?>();
        }
    }

    /// <summary>
    /// Get timezone information for coordinates
    /// </summary>
    [HttpGet(ApiRoutes.Location.Timezone)]
    public async Task<ActionResult<ApiResponse<TimeZoneInfo?>>> GetTimeZone(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetTimeZone), new { Latitude = latitude, Longitude = longitude });
        
        try
        {
            var result = await _azureMapsService.GetTimeZoneAsync(latitude, longitude, cancellationToken);
            LogActionComplete(nameof(GetTimeZone), true);
            return Ok(ApiResponse<TimeZoneInfo?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetTimeZone), new { Latitude = latitude, Longitude = longitude });
            return InternalServerError<TimeZoneInfo?>();
        }
    }

    /// <summary>
    /// Validate and geocode a complete address from components
    /// </summary>
    [HttpPost(ApiRoutes.Location.ValidateAddress)]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> ValidateAddress(
        [FromBody] ValidateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(ValidateAddress), request);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
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
                LogActionComplete(nameof(ValidateAddress), false, "No address components provided");
                return BadRequest(ApiResponse<GeocodeResult?>.ErrorResult("At least one address component is required"));
            }

            var result = await _azureMapsService.GeocodeAddressAsync(fullAddress, cancellationToken);
            
            if (result == null)
            {
                LogActionComplete(nameof(ValidateAddress), false, "Address could not be geocoded");
                return Ok(ApiResponse<GeocodeResult?>.ErrorResult("Address could not be validated or geocoded"));
            }

            LogActionComplete(nameof(ValidateAddress), true);
            return Ok(ApiResponse<GeocodeResult?>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(ValidateAddress), request);
            return InternalServerError<GeocodeResult?>();
        }
    }
}
