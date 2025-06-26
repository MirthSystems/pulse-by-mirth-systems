using Application.Common.Interfaces.Services;
using Application.Common.Models.Location;
using Application.Common.Models;
using Application.Common.Models.Venue;
using Application.Common.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for venue operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;
    private readonly ILogger<VenuesController> _logger;

    public VenuesController(IVenueService venueService, ILogger<VenuesController> logger)
    {
        _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all venues
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetAllVenues(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all venues");
        var result = await _venueService.GetAllVenuesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venue by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Venue?>>> GetVenueById(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venue with ID: {VenueId}", id);
        var result = await _venueService.GetVenueByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new venue
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Venue>>> CreateVenue([FromBody] CreateVenue createVenue, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new venue: {VenueName}", createVenue.Name);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _venueService.CreateVenueAsync(createVenue, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }
        
        return CreatedAtAction(
            nameof(GetVenueById),
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>
    /// Update an existing venue
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Venue?>>> UpdateVenue(long id, [FromBody] UpdateVenue updateVenue, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating venue with ID: {VenueId}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _venueService.UpdateVenueAsync(id, updateVenue, cancellationToken);
        
        if (!result.Success)
        {
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a venue
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVenue(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting venue with ID: {VenueId}", id);
        
        var result = await _venueService.DeleteVenueAsync(id, cancellationToken);
        
        if (!result.Success)
        {
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Get active venues only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetActiveVenues(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active venues");
        var result = await _venueService.GetActiveVenuesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venues by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venues for category: {CategoryId}", categoryId);
        var result = await _venueService.GetVenuesByCategoryAsync(categoryId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venues with active specials
    /// </summary>
    [HttpGet("with-specials")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesWithActiveSpecials(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venues with active specials");
        var result = await _venueService.GetVenuesWithActiveSpecialsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venues near a location
    /// </summary>
    [HttpGet("near")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesNearLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInMeters = 5000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venues near location: {Latitude}, {Longitude} within {Radius}m", 
            latitude, longitude, radiusInMeters);
        
        var result = await _venueService.GetVenuesNearLocationAsync(latitude, longitude, radiusInMeters, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search venues
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<PagedResponse<VenueSummary>>>> SearchVenues(
        [FromBody] VenueSearch search,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching venues with term: {SearchTerm}", search.SearchTerm);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _venueService.SearchVenuesAsync(search, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venue categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueCategory>>>> GetVenueCategories(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venue categories");
        var result = await _venueService.GetVenueCategoriesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get venue category by ID
    /// </summary>
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<ApiResponse<VenueCategory?>>> GetVenueCategoryById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting venue category with ID: {CategoryId}", id);
        var result = await _venueService.GetVenueCategoryByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
    }

    #region Azure Maps Enhanced Endpoints

    /// <summary>
    /// Geocode venue address using Azure Maps
    /// </summary>
    [HttpPost("{id}/geocode")]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> GeocodeVenueAddress(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Geocoding address for venue ID: {VenueId}", id);
        var result = await _venueService.GeocodeVenueAddressAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get enhanced location details for a venue using Azure Maps reverse geocoding
    /// </summary>
    [HttpGet("{id}/location-details")]
    public async Task<ActionResult<ApiResponse<ReverseGeocodeResult?>>> GetVenueLocationDetails(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting location details for venue ID: {VenueId}", id);
        var result = await _venueService.GetVenueLocationDetailsAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get nearby points of interest for a venue using Azure Maps
    /// </summary>
    [HttpGet("{id}/nearby-pois")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PointOfInterest>>>> GetNearbyPointsOfInterest(
        long id, 
        [FromQuery] string? category = null, 
        [FromQuery] int radiusInMeters = 1000, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting nearby POIs for venue ID: {VenueId}, Category: {Category}, Radius: {Radius}m", 
            id, category, radiusInMeters);
        var result = await _venueService.GetNearbyPointsOfInterestAsync(id, category, radiusInMeters, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get timezone information for a venue using Azure Maps
    /// </summary>
    [HttpGet("{id}/timezone")]
    public async Task<ActionResult<ApiResponse<TimeZoneInfo?>>> GetVenueTimeZone(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting timezone for venue ID: {VenueId}", id);
        var result = await _venueService.GetVenueTimeZoneAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Enhanced venue search that combines PostGIS database results with Azure Maps POI data
    /// </summary>
    [HttpPost("search/enhanced")]
    public async Task<ActionResult<ApiResponse<EnhancedVenueSearchResult>>> SearchVenuesWithPOIData(
        [FromBody] VenueSearch search, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Performing enhanced venue search with POI data");
        var result = await _venueService.SearchVenuesWithPOIDataAsync(search, cancellationToken);
        return Ok(result);
    }

    #endregion
}
