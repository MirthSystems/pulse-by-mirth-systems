using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Special;
using Application.Common.Models.Search;
using Application.Common.Utilities;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for special operations
/// </summary>
[ApiController]
public class SpecialsController : BaseApiController
{
    private readonly ISpecialService _specialService;
    private readonly IPermissionService _permissionService;

    public SpecialsController(
        ISpecialService specialService, 
        IPermissionService permissionService,
        IAuthorizationService authorizationService,
        ILogger<SpecialsController> logger)
        : base(logger, authorizationService)
    {
        _specialService = specialService ?? throw new ArgumentNullException(nameof(specialService));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    /// <summary>
    /// Get all specials
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetAll)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetAllSpecials(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetAllSpecials));
        
        try
        {
            var result = await _specialService.GetAllSpecialsAsync(cancellationToken);
            LogActionComplete(nameof(GetAllSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetAllSpecials));
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get special by ID
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetById)]
    public async Task<ActionResult<ApiResponse<Special?>>> GetSpecialById(long id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialById), new { id });
        
        try
        {
            var result = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
            LogActionComplete(nameof(GetSpecialById), result.Success);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialById), new { id });
            return InternalServerError<Special?>();
        }
    }

    /// <summary>
    /// Create a new special
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Special>>> CreateSpecial([FromBody] CreateSpecial createSpecial, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(CreateSpecial), createSpecial);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        try
        {
            var result = await _specialService.CreateSpecialAsync(createSpecial, cancellationToken);
            LogActionComplete(nameof(CreateSpecial), result.Success);
            
            if (result.Success)
            {
                return CreatedAtAction(
                    nameof(GetSpecialById),
                    new { id = result.Data!.Id },
                    result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(CreateSpecial), createSpecial);
            return InternalServerError<Special>();
        }
    }

    /// <summary>
    /// Get active specials only
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetActive)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecials(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetActiveSpecials));
        
        try
        {
            var result = await _specialService.GetActiveSpecialsAsync(cancellationToken);
            LogActionComplete(nameof(GetActiveSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetActiveSpecials));
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get specials by venue
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetByVenue)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsByVenue(long venueId, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialsByVenue), new { venueId });
        
        try
        {
            var result = await _specialService.GetSpecialsByVenueAsync(venueId, cancellationToken);
            LogActionComplete(nameof(GetSpecialsByVenue), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialsByVenue), new { venueId });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get active specials by venue
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetActiveByVenue)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsByVenue(long venueId, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetActiveSpecialsByVenue), new { venueId });
        
        try
        {
            var result = await _specialService.GetActiveSpecialsByVenueAsync(venueId, cancellationToken);
            LogActionComplete(nameof(GetActiveSpecialsByVenue), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetActiveSpecialsByVenue), new { venueId });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get specials by category
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetByCategory)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialsByCategory), new { categoryId });
        
        try
        {
            var result = await _specialService.GetSpecialsByCategoryAsync(categoryId, cancellationToken);
            LogActionComplete(nameof(GetSpecialsByCategory), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialsByCategory), new { categoryId });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get active specials by category
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetActiveByCategoryFull)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsByCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetActiveSpecialsByCategory), new { categoryId });
        
        try
        {
            var result = await _specialService.GetActiveSpecialsByCategoryAsync(categoryId, cancellationToken);
            LogActionComplete(nameof(GetActiveSpecialsByCategory), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetActiveSpecialsByCategory), new { categoryId });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get recurring specials
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetRecurring)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetRecurringSpecials(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetRecurringSpecials));
        
        try
        {
            var result = await _specialService.GetRecurringSpecialsAsync(cancellationToken);
            LogActionComplete(nameof(GetRecurringSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetRecurringSpecials));
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get specials active right now
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetNow)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetSpecialsActiveNow(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialsActiveNow));
        
        try
        {
            var result = await _specialService.GetSpecialsActiveNowAsync(cancellationToken);
            LogActionComplete(nameof(GetSpecialsActiveNow), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialsActiveNow));
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Get active specials near a location
    /// </summary>
    [HttpGet(ApiRoutes.Specials.GetNear)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetActiveSpecialsNearLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInMeters = 5000,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetActiveSpecialsNearLocation), new { latitude, longitude, radiusInMeters });
        
        try
        {
            var result = await _specialService.GetActiveSpecialsNearLocationAsync(latitude, longitude, radiusInMeters, cancellationToken);
            LogActionComplete(nameof(GetActiveSpecialsNearLocation), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetActiveSpecialsNearLocation), new { latitude, longitude, radiusInMeters });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Search specials
    /// </summary>
    [HttpPost(ApiRoutes.Specials.Search)]
    public async Task<ActionResult<ApiResponse<PagedResponse<SpecialSummary>>>> SearchSpecials(
        [FromBody] SpecialSearch search,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SearchSpecials), search);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        try
        {
            var result = await _specialService.SearchSpecialsAsync(search, cancellationToken);
            LogActionComplete(nameof(SearchSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SearchSpecials), search);
            return InternalServerError<PagedResponse<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Enhanced search for venues with categorized specials in an area
    /// Returns venues with active specials organized by food, drink, and entertainment categories
    /// </summary>
    [HttpPost(ApiRoutes.Specials.SearchVenues)]
    public async Task<ActionResult<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>>> SearchVenuesWithSpecials(
        [FromBody] EnhancedSpecialSearch searchRequest,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SearchVenuesWithSpecials), searchRequest);
        
        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;
        
        try
        {
            var result = await _specialService.SearchVenuesWithSpecialsAsync(searchRequest, cancellationToken);
            LogActionComplete(nameof(SearchVenuesWithSpecials), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SearchVenuesWithSpecials), searchRequest);
            return InternalServerError<PagedResponse<VenueWithCategorizedSpecials>>();
        }
    }

    /// <summary>
    /// Get special categories
    /// </summary>
    [HttpGet(ApiRoutes.Specials.Categories)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialCategory>>>> GetSpecialCategories(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialCategories));
        
        try
        {
            var result = await _specialService.GetSpecialCategoriesAsync(cancellationToken);
            LogActionComplete(nameof(GetSpecialCategories), result.Success);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialCategories));
            return InternalServerError<IEnumerable<SpecialCategory>>();
        }
    }

    /// <summary>
    /// Get special category by ID
    /// </summary>
    [HttpGet(ApiRoutes.Specials.CategoryById)]
    public async Task<ActionResult<ApiResponse<SpecialCategory?>>> GetSpecialCategoryById(int id, CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetSpecialCategoryById), new { id });
        
        try
        {
            var result = await _specialService.GetSpecialCategoryByIdAsync(id, cancellationToken);
            LogActionComplete(nameof(GetSpecialCategoryById), result.Success);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetSpecialCategoryById), new { id });
            return InternalServerError<SpecialCategory?>();
        }
    }

    #region Management Endpoints (Require Authorization)

    /// <summary>
    /// Get specials user has access to (filtered by venue permissions)
    /// </summary>
    [HttpGet(ApiRoutes.Specials.MySpecials)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetMySpecials(
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetMySpecials));

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Get accessible venue IDs for the user
            var venueIds = await CleanPermissionHelper.GetAccessibleVenueIdsAsync(
                _permissionService, User, userSub!, cancellationToken);

            // Get specials for accessible venues
            var specials = new List<SpecialSummary>();
            foreach (var venueId in venueIds)
            {
                var venueSpecials = await _specialService.GetSpecialsByVenueAsync(venueId, cancellationToken);
                if (venueSpecials.Success && venueSpecials.Data != null)
                {
                    specials.AddRange(venueSpecials.Data);
                }
            }

            var response = ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specials);
            LogActionComplete(nameof(GetMySpecials), true);
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMySpecials), new { UserSub = userSub });
            return InternalServerError<IEnumerable<SpecialSummary>>();
        }
    }

    /// <summary>
    /// Create a new special (requires venue access)
    /// </summary>
    [HttpPost(ApiRoutes.Specials.Create)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Special>>> CreateSpecialForVenue(
        [FromRoute] long venueId,
        [FromBody] CreateSpecial createSpecial,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(CreateSpecialForVenue), new { VenueId = venueId, CreateSpecial = createSpecial });

        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Ensure the venue ID matches the route
            createSpecial.VenueId = venueId;

            // Check venue access permissions for the special's venue
            var hasAccess = await CleanPermissionHelper.HasVenueAccessAsync(
                _permissionService, User, userSub!, createSpecial.VenueId, cancellationToken);
            
            if (!hasAccess)
            {
                LogError(new UnauthorizedAccessException("User attempted to create special without venue access"), 
                    nameof(CreateSpecialForVenue), new { UserSub = userSub, VenueId = createSpecial.VenueId });
                return Forbid();
            }

            // Create the special
            var result = await _specialService.CreateSpecialAsync(createSpecial, cancellationToken);
            
            LogActionComplete(nameof(CreateSpecialForVenue), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(CreateSpecialForVenue), new { UserSub = userSub, VenueId = venueId, CreateSpecial = createSpecial });
            return InternalServerError<Special>();
        }
    }

    /// <summary>
    /// Update an existing special (requires venue access)
    /// </summary>
    [HttpPut(ApiRoutes.Specials.Update)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Special>>> UpdateSpecial(
        [FromRoute] long id,
        [FromBody] UpdateSpecial updateSpecial,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(UpdateSpecial), new { Id = id, UpdateSpecial = updateSpecial });

        // Validate model state
        var modelValidation = ValidateModelState();
        if (modelValidation != null) return modelValidation;

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Get the special to check venue access
            var specialResult = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
            if (!specialResult.Success || specialResult.Data == null)
            {
                return NotFound();
            }

            // Check venue access permissions
            var hasAccess = await CleanPermissionHelper.HasVenueAccessAsync(
                _permissionService, User, userSub!, specialResult.Data.VenueId, cancellationToken);
            
            if (!hasAccess)
            {
                LogError(new UnauthorizedAccessException("User attempted to update special without venue access"), 
                    nameof(UpdateSpecial), new { UserSub = userSub, SpecialId = id });
                return Forbid();
            }

            // Update the special
            var result = await _specialService.UpdateSpecialAsync(id, updateSpecial, cancellationToken);
            
            LogActionComplete(nameof(UpdateSpecial), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(UpdateSpecial), new { Id = id, UserSub = userSub, UpdateSpecial = updateSpecial });
            return InternalServerError<Special>();
        }
    }

    /// <summary>
    /// Delete a special (requires venue access)
    /// </summary>
    [HttpDelete(ApiRoutes.Specials.Delete)]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSpecial(
        [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(DeleteSpecial), new { Id = id });

        // Validate backoffice access and get user context
        var (isAuthorized, userSub, authError) = await ValidateBackofficeAccessAsync();
        
        if (authError != null) return authError;

        try
        {
            // Get the special to check venue access
            var specialResult = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
            if (!specialResult.Success || specialResult.Data == null)
            {
                return NotFound();
            }

            // Check venue access permissions
            var hasAccess = await CleanPermissionHelper.HasVenueAccessAsync(
                _permissionService, User, userSub!, specialResult.Data.VenueId, cancellationToken);
            
            if (!hasAccess)
            {
                LogError(new UnauthorizedAccessException("User attempted to delete special without venue access"), 
                    nameof(DeleteSpecial), new { UserSub = userSub, SpecialId = id });
                return Forbid();
            }

            // Delete the special
            var result = await _specialService.DeleteSpecialAsync(id, cancellationToken);
            
            LogActionComplete(nameof(DeleteSpecial), result.Success);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(DeleteSpecial), new { Id = id, UserSub = userSub });
            return InternalServerError<bool>();
        }
    }

    #endregion
}
