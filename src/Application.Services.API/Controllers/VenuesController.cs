using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models.Location;
using Application.Common.Models;
using Application.Common.Models.Venue;
using Application.Common.Models.Search;
using Application.Common.Utilities;
using Application.Infrastructure.Authorization.Requirements;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for venue operations
/// </summary>
[ApiController]
public class VenuesController : BaseApiController
{
    private readonly IVenueService _venueService;
    private readonly IPermissionService _permissionService;

    public VenuesController(
        IVenueService venueService, 
        IPermissionService permissionService,
        IAuthorizationService authorizationService,
        ILogger<VenuesController> logger)
        : base(logger, authorizationService)
    {
        _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    /// <summary>
    /// Get all venues
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetAll)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetAllVenues(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetAllVenues));
        
        try
        {
            var result = await _venueService.GetAllVenuesAsync(cancellationToken);
            LogActionComplete(nameof(GetAllVenues), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetAllVenues));
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Get venue by ID
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetById)]
    public async Task<ActionResult<ApiResponse<Venue?>>> GetVenueById(long id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueById), new { VenueId = id });
        
        try
        {
            var result = await _venueService.GetVenueByIdAsync(id, cancellationToken);
            
            if (!result.Success || result.Data == null)
            {
                LogActionComplete(nameof(GetVenueById), false, "Venue not found");
                return NotFound(result);
            }
            
            LogActionComplete(nameof(GetVenueById), true);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueById), new { VenueId = id });
            return InternalServerError<Venue?>();
        }
    }

    /// <summary>
    /// Get active venues only
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetActive)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetActiveVenues(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetActiveVenues));
        
        try
        {
            var result = await _venueService.GetActiveVenuesAsync(cancellationToken);
            LogActionComplete(nameof(GetActiveVenues), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetActiveVenues));
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Get venues by category
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetByCategory)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenuesByCategory), new { CategoryId = categoryId });
        
        try
        {
            var result = await _venueService.GetVenuesByCategoryAsync(categoryId, cancellationToken);
            LogActionComplete(nameof(GetVenuesByCategory), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenuesByCategory), new { CategoryId = categoryId });
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Get venues with active specials
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetWithSpecials)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesWithActiveSpecials(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenuesWithActiveSpecials));
        
        try
        {
            var result = await _venueService.GetVenuesWithActiveSpecialsAsync(cancellationToken);
            LogActionComplete(nameof(GetVenuesWithActiveSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenuesWithActiveSpecials));
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Get venues near a location
    /// </summary>
    [HttpGet(ApiRoutes.Venues.GetNear)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetVenuesNearLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInMeters = 5000,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenuesNearLocation), new { Latitude = latitude, Longitude = longitude, RadiusInMeters = radiusInMeters });
        
        try
        {
            var result = await _venueService.GetVenuesNearLocationAsync(latitude, longitude, radiusInMeters, cancellationToken);
            LogActionComplete(nameof(GetVenuesNearLocation), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenuesNearLocation), new { Latitude = latitude, Longitude = longitude, RadiusInMeters = radiusInMeters });
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Search venues
    /// </summary>
    [HttpPost(ApiRoutes.Venues.Search)]
    public async Task<ActionResult<ApiResponse<PagedResponse<VenueSummary>>>> SearchVenues(
        [FromBody] VenueSearch search,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SearchVenues), search);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        try
        {
            var result = await _venueService.SearchVenuesAsync(search, cancellationToken);
            LogActionComplete(nameof(SearchVenues), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SearchVenues), search);
            return InternalServerError<PagedResponse<VenueSummary>>();
        }
    }

    /// <summary>
    /// Get venue categories
    /// </summary>
    [HttpGet(ApiRoutes.Venues.Categories)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueCategory>>>> GetVenueCategories(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueCategories));
        
        try
        {
            var result = await _venueService.GetVenueCategoriesAsync(cancellationToken);
            LogActionComplete(nameof(GetVenueCategories), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueCategories));
            return InternalServerError<IEnumerable<VenueCategory>>();
        }
    }

    /// <summary>
    /// Get venue category by ID
    /// </summary>
    [HttpGet(ApiRoutes.Venues.CategoryById)]
    public async Task<ActionResult<ApiResponse<VenueCategory?>>> GetVenueCategoryById(int id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueCategoryById), new { CategoryId = id });
        
        try
        {
            var result = await _venueService.GetVenueCategoryByIdAsync(id, cancellationToken);
            
            if (!result.Success || result.Data == null)
            {
                LogActionComplete(nameof(GetVenueCategoryById), false, "Category not found");
                return NotFound(result);
            }
            
            LogActionComplete(nameof(GetVenueCategoryById), true);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueCategoryById), new { CategoryId = id });
            return InternalServerError<VenueCategory?>();
        }
    }

    #region Azure Maps Enhanced Endpoints

    /// <summary>
    /// Geocode venue address using Azure Maps
    /// </summary>
    [HttpPost(ApiRoutes.Venues.Geocode)]
    public async Task<ActionResult<ApiResponse<GeocodeResult?>>> GeocodeVenueAddress(long id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GeocodeVenueAddress), new { VenueId = id });
        
        try
        {
            var result = await _venueService.GeocodeVenueAddressAsync(id, cancellationToken);
            LogActionComplete(nameof(GeocodeVenueAddress), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GeocodeVenueAddress), new { VenueId = id });
            return InternalServerError<GeocodeResult?>();
        }
    }

    /// <summary>
    /// Get enhanced location details for a venue using Azure Maps reverse geocoding
    /// </summary>
    [HttpGet(ApiRoutes.Venues.LocationDetails)]
    public async Task<ActionResult<ApiResponse<ReverseGeocodeResult?>>> GetVenueLocationDetails(long id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueLocationDetails), new { VenueId = id });
        
        try
        {
            var result = await _venueService.GetVenueLocationDetailsAsync(id, cancellationToken);
            LogActionComplete(nameof(GetVenueLocationDetails), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueLocationDetails), new { VenueId = id });
            return InternalServerError<ReverseGeocodeResult?>();
        }
    }

    /// <summary>
    /// Get nearby points of interest for a venue using Azure Maps
    /// </summary>
    [HttpGet(ApiRoutes.Venues.NearbyPOIs)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PointOfInterest>>>> GetNearbyPointsOfInterest(
        long id, 
        [FromQuery] string? category = null, 
        [FromQuery] int radiusInMeters = 1000, 
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetNearbyPointsOfInterest), new { VenueId = id, Category = category, RadiusInMeters = radiusInMeters });
        
        try
        {
            var result = await _venueService.GetNearbyPointsOfInterestAsync(id, category, radiusInMeters, cancellationToken);
            LogActionComplete(nameof(GetNearbyPointsOfInterest), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetNearbyPointsOfInterest), new { VenueId = id, Category = category, RadiusInMeters = radiusInMeters });
            return InternalServerError<IEnumerable<PointOfInterest>>();
        }
    }

    /// <summary>
    /// Get timezone information for a venue using Azure Maps
    /// </summary>
    [HttpGet(ApiRoutes.Venues.Timezone)]
    public async Task<ActionResult<ApiResponse<TimeZoneInfo?>>> GetVenueTimeZone(long id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueTimeZone), new { VenueId = id });
        
        try
        {
            var result = await _venueService.GetVenueTimeZoneAsync(id, cancellationToken);
            LogActionComplete(nameof(GetVenueTimeZone), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueTimeZone), new { VenueId = id });
            return InternalServerError<TimeZoneInfo?>();
        }
    }

    /// <summary>
    /// Enhanced venue search that combines PostGIS database results with Azure Maps POI data
    /// </summary>
    [HttpPost(ApiRoutes.Venues.SearchEnhanced)]
    public async Task<ActionResult<ApiResponse<EnhancedVenueSearchResult>>> SearchVenuesWithPOIData(
        [FromBody] VenueSearch search, 
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SearchVenuesWithPOIData), search);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        try
        {
            var result = await _venueService.SearchVenuesWithPOIDataAsync(search, cancellationToken);
            LogActionComplete(nameof(SearchVenuesWithPOIData), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SearchVenuesWithPOIData), search);
            return InternalServerError<EnhancedVenueSearchResult>();
        }
    }

    #endregion

    #region Management Endpoints (Require Authorization)

    /// <summary>
    /// Get venues user has access to (filtered by permissions)
    /// </summary>
    [HttpGet(ApiRoutes.Venues.MyVenues)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetMyVenues(
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetMyVenues));

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Get accessible venue IDs for the user
            var venueIds = await PermissionUtils.GetAccessibleVenueIdsAsync(
                _permissionService, User, userSub!, cancellationToken);

            // Get venue details for accessible venues
            var venues = new List<VenueSummary>();
            foreach (var venueId in venueIds)
            {
                var venueResult = await _venueService.GetVenueByIdAsync(venueId, cancellationToken);
                if (venueResult.Success && venueResult.Data != null)
                {
                    venues.Add(new VenueSummary
                    {
                        Id = venueResult.Data.Id,
                        Name = venueResult.Data.Name,
                        Description = venueResult.Data.Description,
                        CategoryId = venueResult.Data.CategoryId,
                        CategoryName = venueResult.Data.Category?.Name ?? "",
                        CategoryIcon = venueResult.Data.Category?.Icon ?? "",
                        ActiveSpecialsCount = venueResult.Data.ActiveSpecials?.Count ?? 0,
                        StreetAddress = venueResult.Data.StreetAddress,
                        Locality = venueResult.Data.Locality,
                        Region = venueResult.Data.Region,
                        PostalCode = venueResult.Data.PostalCode,
                        Country = venueResult.Data.Country,
                        IsActive = venueResult.Data.IsActive
                    });
                }
            }

            var response = ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(venues);
            LogActionComplete(nameof(GetMyVenues), true);
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMyVenues), new { UserSub = userSub });
            return InternalServerError<IEnumerable<VenueSummary>>();
        }
    }

    /// <summary>
    /// Create a new venue (requires system admin or content manager permissions)
    /// </summary>
    [HttpPost(ApiRoutes.Venues.Create)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Venue>>> CreateVenue(
        [FromBody] CreateVenue createVenue,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(CreateVenue), createVenue);

        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Validate venue creation permissions (admin/content manager only)
            if (!PermissionUtils.CanCreateVenues(User))
            {
                LogError(new UnauthorizedAccessException("User attempted to create venue without proper permissions"), 
                    nameof(CreateVenue), new { UserSub = userSub });
                return Forbid();
            }

            // Create the venue
            var result = await _venueService.CreateVenueAsync(createVenue, cancellationToken);
            
            LogActionComplete(nameof(CreateVenue), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(CreateVenue), new { UserSub = userSub, CreateVenue = createVenue });
            return InternalServerError<Venue>();
        }
    }

    /// <summary>
    /// Update an existing venue (requires venue access)
    /// </summary>
    [HttpPut(ApiRoutes.Venues.Update)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Venue>>> UpdateVenue(
        [FromRoute] long id,
        [FromBody] UpdateVenue updateVenue,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(UpdateVenue), new { Id = id, UpdateVenue = updateVenue });

        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Check venue access permissions
            var hasAccess = await PermissionUtils.HasVenueAccessAsync(
                _permissionService, User, userSub!, id, cancellationToken);
            
            if (!hasAccess)
            {
                LogError(new UnauthorizedAccessException("User attempted to update venue without access"), 
                    nameof(UpdateVenue), new { UserSub = userSub, VenueId = id });
                return Forbid();
            }

            // Update the venue
            var result = await _venueService.UpdateVenueAsync(id, updateVenue, cancellationToken);
            
            LogActionComplete(nameof(UpdateVenue), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(UpdateVenue), new { Id = id, UserSub = userSub, UpdateVenue = updateVenue });
            return InternalServerError<Venue>();
        }
    }

    /// <summary>
    /// Delete a venue (requires venue access)
    /// </summary>
    [HttpDelete(ApiRoutes.Venues.Delete)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVenue(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(DeleteVenue), new { Id = id });

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Check venue access permissions
            var hasAccess = await PermissionUtils.HasVenueAccessAsync(
                _permissionService, User, userSub!, id, cancellationToken);
            
            if (!hasAccess)
            {
                LogError(new UnauthorizedAccessException("User attempted to delete venue without access"), 
                    nameof(DeleteVenue), new { UserSub = userSub, VenueId = id });
                return Forbid();
            }

            // Delete the venue
            var result = await _venueService.DeleteVenueAsync(id, cancellationToken);
            
            LogActionComplete(nameof(DeleteVenue), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(DeleteVenue), new { Id = id, UserSub = userSub });
            return InternalServerError<bool>();
        }
    }

    #endregion
}
