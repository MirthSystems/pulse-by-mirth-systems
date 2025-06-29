using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Special;
using Application.Common.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for special operations
/// </summary>
[ApiController]
[Route("api/specials")]
public class SpecialsController : ControllerBase
{
    private readonly ISpecialService _specialService;
    private readonly ILogger<SpecialsController> _logger;

    public SpecialsController(ISpecialService specialService, ILogger<SpecialsController> logger)
    {
        _specialService = specialService ?? throw new ArgumentNullException(nameof(specialService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all specials
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetAllSpecials(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all specials");
        var result = await _specialService.GetAllSpecialsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get special by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Special?>>> GetSpecialById(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting special with ID: {SpecialId}", id);
        var result = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new special
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Special>>> CreateSpecial([FromBody] CreateSpecial createSpecial, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new special: {SpecialTitle}", createSpecial.Title);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _specialService.CreateSpecialAsync(createSpecial, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }
        
        return CreatedAtAction(
            nameof(GetSpecialById),
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>
    /// Get active specials only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecials(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active specials");
        var result = await _specialService.GetActiveSpecialsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get specials by venue
    /// </summary>
    [HttpGet("venue/{venueId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsByVenue(long venueId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting specials for venue: {VenueId}", venueId);
        var result = await _specialService.GetSpecialsByVenueAsync(venueId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get active specials by venue
    /// </summary>
    [HttpGet("venue/{venueId}/active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsByVenue(long venueId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active specials for venue: {VenueId}", venueId);
        var result = await _specialService.GetActiveSpecialsByVenueAsync(venueId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get specials by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting specials for category: {CategoryId}", categoryId);
        var result = await _specialService.GetSpecialsByCategoryAsync(categoryId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get active specials by category
    /// </summary>
    [HttpGet("category/{categoryId}/active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active specials for category: {CategoryId}", categoryId);
        var result = await _specialService.GetActiveSpecialsByCategoryAsync(categoryId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get recurring specials
    /// </summary>
    [HttpGet("recurring")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetRecurringSpecials(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting recurring specials");
        var result = await _specialService.GetRecurringSpecialsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get specials active right now
    /// </summary>
    [HttpGet("now")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsActiveNow(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting specials active now");
        var result = await _specialService.GetSpecialsActiveNowAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get active specials near a location
    /// </summary>
    [HttpGet("near")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsNearLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInMeters = 5000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active specials near location: {Latitude}, {Longitude} within {Radius}m", 
            latitude, longitude, radiusInMeters);
        
        var result = await _specialService.GetActiveSpecialsNearLocationAsync(latitude, longitude, radiusInMeters, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search specials
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<PagedResponse<SpecialSummary>>>> SearchSpecials(
        [FromBody] SpecialSearch search,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching specials with term: {SearchTerm}", search.SearchTerm);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _specialService.SearchSpecialsAsync(search, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Enhanced search for venues with categorized specials in an area
    /// Returns venues with active specials organized by food, drink, and entertainment categories
    /// </summary>
    [HttpPost("search/venues")]
    public async Task<ActionResult<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>>> SearchVenuesWithSpecials(
        [FromBody] EnhancedSpecialSearch searchRequest,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching venues with specials near location: {Latitude}, {Longitude} within {Radius}m", 
            searchRequest.Latitude, searchRequest.Longitude, searchRequest.RadiusInMeters);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _specialService.SearchVenuesWithSpecialsAsync(searchRequest, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get special categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialCategory>>>> GetSpecialCategories(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting special categories");
        var result = await _specialService.GetSpecialCategoriesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get special category by ID
    /// </summary>
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<ApiResponse<SpecialCategory?>>> GetSpecialCategoryById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting special category with ID: {CategoryId}", id);
        var result = await _specialService.GetSpecialCategoryByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
    }
}
