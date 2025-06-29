using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Special;
using Application.Common.Models.Venue;
using Application.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for backoffice operations (venue management, staff functions)
/// All endpoints require backoffice access (Auth0 permissions OR venue permissions)
/// Data is filtered based on user's venue access
/// </summary>
[ApiController]
[Route("api/backoffice")]
[Authorize] // Require authentication
public class BackofficeController : ControllerBase
{
    private readonly IVenueService _venueService;
    private readonly ISpecialService _specialService;
    private readonly IPermissionService _permissionService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<BackofficeController> _logger;

    public BackofficeController(
        IVenueService venueService,
        ISpecialService specialService,
        IPermissionService permissionService,
        IAuthorizationService authorizationService,
        ILogger<BackofficeController> logger)
    {
        _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        _specialService = specialService ?? throw new ArgumentNullException(nameof(specialService));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get venues user has access to (filtered by permissions)
    /// </summary>
    [HttpGet("venues")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetMyVenues(CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Get venues based on user's permission level
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            var userVenueIds = await _permissionService.GetUserAccessibleVenueIdsAsync(userSub, isSystemAdmin, isContentManager, cancellationToken);
            var venues = new List<VenueSummary>();

            foreach (var venueId in userVenueIds)
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

            return Ok(ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(venues));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venues for user {UserSub}", userSub);
            return StatusCode(500, ApiResponse<IEnumerable<VenueSummary>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get specials user has access to (filtered by venue permissions)
    /// </summary>
    [HttpGet("specials")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecialSummary>>>> GetMySpecials(CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Get specials based on user's permission level
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            var userVenueIds = await _permissionService.GetUserAccessibleVenueIdsAsync(userSub, isSystemAdmin, isContentManager, cancellationToken);
            var specials = new List<SpecialSummary>();

            foreach (var venueId in userVenueIds)
            {
                var venueSpecials = await _specialService.GetSpecialsByVenueAsync(venueId, cancellationToken);
                if (venueSpecials.Success && venueSpecials.Data != null)
                {
                    specials.AddRange(venueSpecials.Data);
                }
            }

            return Ok(ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specials));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting specials for user {UserSub}", userSub);
            return StatusCode(500, ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Create a special (must have permission for the venue)
    /// </summary>
    [HttpPost("venues/{venueId}/specials")]
    public async Task<ActionResult<ApiResponse<Special>>> CreateSpecial(
        long venueId,
        [FromBody] CreateSpecial createSpecial,
        CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Check if user can manage this specific venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, venueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            // Override the venue ID to ensure security
            createSpecial.VenueId = venueId;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating special for venue {VenueId}", venueId);
            return StatusCode(500, ApiResponse<Special>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Update a special (must have permission for the venue)
    /// </summary>
    [HttpPut("specials/{id}")]
    public async Task<ActionResult<ApiResponse<Special?>>> UpdateSpecial(
        long id,
        [FromBody] UpdateSpecial updateSpecial,
        CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Get the special to check venue ownership
            var specialResult = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
            if (!specialResult.Success || specialResult.Data == null)
            {
                return NotFound(specialResult);
            }

            var venueId = specialResult.Data.VenueId;

            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, venueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var result = await _specialService.UpdateSpecialAsync(id, updateSpecial, cancellationToken);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating special {SpecialId}", id);
            return StatusCode(500, ApiResponse<Special>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Delete a special (must have permission for the venue)
    /// </summary>
    [HttpDelete("specials/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSpecial(long id, CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Get the special to check venue ownership
            var specialResult = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
            if (!specialResult.Success || specialResult.Data == null)
            {
                return NotFound(specialResult);
            }

            var venueId = specialResult.Data.VenueId;

            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, venueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var result = await _specialService.DeleteSpecialAsync(id, cancellationToken);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting special {SpecialId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get special by ID (helper method for created response)
    /// </summary>
    private async Task<ActionResult<ApiResponse<Special?>>> GetSpecialById(long id, CancellationToken cancellationToken = default)
    {
        var result = await _specialService.GetSpecialByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
    }
}
