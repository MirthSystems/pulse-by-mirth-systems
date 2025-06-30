using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Common.Models.Special;
using Application.Common.Models.Venue;
using Application.Infrastructure.Authorization.Requirements;
using Application.Infrastructure.Authorization.Permissions;
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
    private readonly IVenuePermissionTypeService _permissionTypeService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<BackofficeController> _logger;
    private readonly IConfiguration _configuration;

    public BackofficeController(
        IVenueService venueService,
        ISpecialService specialService,
        IPermissionService permissionService,
        IVenuePermissionTypeService permissionTypeService,
        IAuthorizationService authorizationService,
        ILogger<BackofficeController> logger,
        IConfiguration configuration)
    {
        _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
        _specialService = specialService ?? throw new ArgumentNullException(nameof(specialService));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _permissionTypeService = permissionTypeService ?? throw new ArgumentNullException(nameof(permissionTypeService));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Get venues user has access to (filtered by permissions)
    /// </summary>
    [HttpGet("venues")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetMyVenues(CancellationToken cancellationToken = default)
    {
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
    /// Create a new venue (requires system admin or content manager permissions)
    /// </summary>
    [HttpPost("venues")]
    public async Task<ActionResult<ApiResponse<Venue>>> CreateVenue(
        [FromBody] CreateVenue createVenue,
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
            // Only system admins and content managers can create venues
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                return Forbid();
            }

            var result = await _venueService.CreateVenueAsync(createVenue, cancellationToken);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return StatusCode(201, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating venue");
            return StatusCode(500, ApiResponse<Venue>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Update a venue (must have permission for the venue)
    /// </summary>
    [HttpPut("venues/{id}")]
    public async Task<ActionResult<ApiResponse<Venue?>>> UpdateVenue(
        long id,
        [FromBody] UpdateVenue updateVenue,
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
            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, id, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var result = await _venueService.UpdateVenueAsync(id, updateVenue, cancellationToken);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating venue {VenueId}", id);
            return StatusCode(500, ApiResponse<Venue>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Delete a venue (must have permission for the venue)
    /// </summary>
    [HttpDelete("venues/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVenue(long id, CancellationToken cancellationToken = default)
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
            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, id, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var result = await _venueService.DeleteVenueAsync(id, cancellationToken);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting venue {VenueId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get venue by ID (helper method for created response)
    /// </summary>
    private async Task<ActionResult<ApiResponse<Venue?>>> GetVenueById(long id, CancellationToken cancellationToken = default)
    {
        var result = await _venueService.GetVenueByIdAsync(id, cancellationToken);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(result);
        }
        
        return Ok(result);
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

            return StatusCode(201, result);
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

    /// <summary>
    /// Get current user's venue permissions
    /// </summary>
    [HttpGet("my-permissions")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenuePermissionSummary>>>> GetMyPermissions(CancellationToken cancellationToken = default)
    {
        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            var permissions = await _permissionService.GetUserVenuePermissionsAsync(userSub, cancellationToken);
            return Ok(ApiResponse<IEnumerable<VenuePermissionSummary>>.SuccessResult(permissions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user venue permissions for user {UserSub}", userSub);
            return StatusCode(500, ApiResponse<IEnumerable<VenuePermissionSummary>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get permissions for a specific venue
    /// </summary>
    [HttpGet("venues/{venueId}/permissions")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenuePermissionResponse>>>> GetVenuePermissions(
        long venueId,
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

            var permissions = await _permissionService.GetVenuePermissionsAsync(venueId, cancellationToken);
            
            // Transform to response model
            var result = permissions.Select(p => new VenuePermissionResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                VenueId = p.VenueId,
                VenueName = p.Venue?.Name ?? "",
                Name = p.Name,
                UserEmail = p.User?.Email ?? "",
                GrantedByUserId = p.GrantedByUserId,
                GrantedByUserEmail = p.GrantedByUser?.Email ?? "",
                GrantedAt = p.GrantedAt.ToDateTimeUtc(),
                IsActive = p.IsActive,
                Notes = p.Notes
            });

            return Ok(ApiResponse<IEnumerable<VenuePermissionResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue permissions for venue {VenueId}", venueId);
            return StatusCode(500, ApiResponse<IEnumerable<VenuePermissionResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get invitations for a specific venue
    /// </summary>
    [HttpGet("venues/{venueId}/invitations")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueInvitationResponse>>>> GetVenueInvitations(
        long venueId,
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

            var invitations = await _permissionService.GetVenueInvitationsAsync(venueId, cancellationToken);
            
            _logger.LogInformation("Retrieved {Count} invitations for venue {VenueId}", invitations.Count(), venueId);
            
            // Transform to response model
            var result = invitations.Select(i => new VenueInvitationResponse
            {
                Id = i.Id,
                Email = i.Email,
                VenueId = i.VenueId,
                VenueName = i.Venue?.Name ?? "",
                Permission = i.Permission,
                InvitedByUserId = i.InvitedByUserId,
                InvitedByUserEmail = i.InvitedByUser?.Email ?? "",
                InvitedAt = i.InvitedAt.ToDateTimeUtc(),
                ExpiresAt = i.ExpiresAt.ToDateTimeUtc(),
                AcceptedAt = i.AcceptedAt?.ToDateTimeUtc(),
                AcceptedByUserId = i.AcceptedByUserId,
                IsActive = i.IsActive,
                Notes = i.Notes
            });

            return Ok(ApiResponse<IEnumerable<VenueInvitationResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting venue invitations for venue {VenueId}", venueId);
            return StatusCode(500, ApiResponse<IEnumerable<VenueInvitationResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Send an invitation to join venue management
    /// </summary>
    [HttpPost("invitations")]
    public async Task<ActionResult<ApiResponse<VenueInvitationResponse>>> SendInvitation(
        [FromBody] CreateInvitationRequest invitationRequest,
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
            // Use the sender's email from the request (passed from frontend)
            var senderEmail = invitationRequest.SenderEmail;
            
            // Ensure the inviting user exists in our database with their email
            await _permissionService.EnsureUserExistsAsync(userSub, senderEmail, cancellationToken: cancellationToken);

            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VenueInvitationResponse>.ErrorResult("Invalid invitation data"));
            }

            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, invitationRequest.VenueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            // Validate permission type using the service
            if (!await _permissionTypeService.IsValidPermissionTypeAsync(invitationRequest.Permission, cancellationToken))
            {
                return BadRequest(ApiResponse<VenueInvitationResponse>.ErrorResult("Invalid permission type"));
            }

            // Check if venue exists
            var venueResult = await _venueService.GetVenueByIdAsync(invitationRequest.VenueId, cancellationToken);
            if (!venueResult.Success || venueResult.Data == null)
            {
                return NotFound(ApiResponse<VenueInvitationResponse>.ErrorResult("Venue not found"));
            }

            // Check if user already has permission for this venue
            var existingPermission = await _permissionService.GetUserVenuePermissionAsync(
                invitationRequest.Email, invitationRequest.VenueId, cancellationToken);
            
            if (existingPermission != null)
            {
                return BadRequest(ApiResponse<VenueInvitationResponse>.ErrorResult("User already has permission for this venue"));
            }

            // Check if there's already a pending invitation
            var existingInvitation = await _permissionService.GetPendingInvitationAsync(
                invitationRequest.Email, invitationRequest.VenueId, cancellationToken);
            
            if (existingInvitation != null)
            {
                return BadRequest(ApiResponse<VenueInvitationResponse>.ErrorResult("Invitation already pending for this user"));
            }

            // Create the invitation
            _logger.LogInformation("Creating invitation for {Email} to venue {VenueId} with permission {Permission} from sender {SenderEmail}", 
                invitationRequest.Email, invitationRequest.VenueId, invitationRequest.Permission, senderEmail);
            
            var invitation = await _permissionService.CreateInvitationAsync(invitationRequest, userSub, senderEmail, cancellationToken);
            
            if (invitation == null)
            {
                _logger.LogError("Failed to create invitation for {Email} to venue {VenueId}", invitationRequest.Email, invitationRequest.VenueId);
                return StatusCode(500, ApiResponse<VenueInvitationResponse>.ErrorResult("Failed to create invitation"));
            }

            _logger.LogInformation("Successfully created invitation {InvitationId} for {Email} to venue {VenueId}", 
                invitation.Id, invitationRequest.Email, invitationRequest.VenueId);

            // Create response
            var response = new VenueInvitationResponse
            {
                Id = invitation.Id,
                Email = invitation.Email,
                VenueId = invitation.VenueId,
                VenueName = invitation.Venue?.Name ?? venueResult.Data.Name,
                Permission = invitation.Permission,
                InvitedByUserId = invitation.InvitedByUserId,
                InvitedByUserEmail = invitation.InvitedByUser?.Email ?? User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "",
                InvitedAt = invitation.InvitedAt.ToDateTimeUtc(),
                ExpiresAt = invitation.ExpiresAt.ToDateTimeUtc(),
                AcceptedAt = invitation.AcceptedAt?.ToDateTimeUtc(),
                AcceptedByUserId = invitation.AcceptedByUserId,
                IsActive = invitation.IsActive,
                Notes = invitation.Notes
            };

            _logger.LogInformation("Invitation sent to {Email} for venue {VenueId} with permission {Permission}", 
                invitationRequest.Email, invitationRequest.VenueId, invitationRequest.Permission);

            return Ok(ApiResponse<VenueInvitationResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invitation for venue {VenueId} to {Email}", 
                invitationRequest.VenueId, invitationRequest.Email);
            return StatusCode(500, ApiResponse<VenueInvitationResponse>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get current user's pending invitations
    /// </summary>
    [HttpGet("my-invitations")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueInvitationResponse>>>> GetMyInvitations(
        [FromQuery] string? email = null,
        CancellationToken cancellationToken = default)
    {
        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Use email from query parameter if provided, otherwise try to get from database
            string? userEmail = email;
            
            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = await _permissionService.GetUserEmailBySubAsync(userSub, cancellationToken);
            }
            
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("No user email available for user {UserSub}", userSub);
                return BadRequest(ApiResponse<IEnumerable<VenueInvitationResponse>>.ErrorResult("User email not available"));
            }

            var invitations = await _permissionService.GetUserPendingInvitationsAsync(userEmail, cancellationToken);
            
            // Transform to response model
            var result = invitations.Select(i => new VenueInvitationResponse
            {
                Id = i.Id,
                Email = i.Email,
                VenueId = i.VenueId,
                VenueName = i.Venue?.Name ?? "",
                Permission = i.Permission,
                InvitedByUserId = i.InvitedByUserId,
                InvitedByUserEmail = i.InvitedByUser?.Email ?? "",
                InvitedAt = i.InvitedAt.ToDateTimeUtc(),
                ExpiresAt = i.ExpiresAt.ToDateTimeUtc(),
                AcceptedAt = i.AcceptedAt?.ToDateTimeUtc(),
                AcceptedByUserId = i.AcceptedByUserId,
                IsActive = i.IsActive,
                Notes = i.Notes
            });

            return Ok(ApiResponse<IEnumerable<VenueInvitationResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user invitations for user {UserSub}", userSub);
            return StatusCode(500, ApiResponse<IEnumerable<VenueInvitationResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Accept an invitation
    /// </summary>
    [HttpPost("invitations/{invitationId}/accept")]
    public async Task<ActionResult<ApiResponse<bool>>> AcceptInvitation(long invitationId, CancellationToken cancellationToken = default)
    {
        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Try to get email from claims first, then database
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
                           ?? User.FindFirst("email")?.Value 
                           ?? User.FindFirst("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            
            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = await _permissionService.GetUserEmailBySubAsync(userSub, cancellationToken);
            }
            
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Cannot accept invitation: Your user account does not have an email address. Please contact support."));
            }

            // Ensure user exists in database
            await _permissionService.EnsureUserExistsAsync(userSub, userEmail, cancellationToken: cancellationToken);

            var success = await _permissionService.AcceptInvitationAsync(invitationId, userSub, userEmail, cancellationToken);
            
            if (!success)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Failed to accept invitation"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting invitation {InvitationId} for user {UserSub}", invitationId, userSub);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Decline an invitation
    /// </summary>
    [HttpPost("invitations/{invitationId}/decline")]
    public async Task<ActionResult<ApiResponse<bool>>> DeclineInvitation(long invitationId, CancellationToken cancellationToken = default)
    {
        var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Try to get email from claims first, then database
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
                           ?? User.FindFirst("email")?.Value 
                           ?? User.FindFirst("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            
            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = await _permissionService.GetUserEmailBySubAsync(userSub, cancellationToken);
            }
            
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Cannot decline invitation: Your user account does not have an email address. Please contact support."));
            }

            // Ensure user exists in database
            await _permissionService.EnsureUserExistsAsync(userSub, userEmail, cancellationToken: cancellationToken);

            var success = await _permissionService.DeclineInvitationAsync(invitationId, userSub, userEmail, cancellationToken);
            
            if (!success)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Failed to decline invitation"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error declining invitation {InvitationId} for user {UserSub}", invitationId, userSub);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Cancel/revoke an invitation (for venue owners/managers)
    /// </summary>
    [HttpDelete("invitations/{invitationId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelInvitation(long invitationId, CancellationToken cancellationToken = default)
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
            // Get the invitation to check permissions
            var invitation = await _permissionService.GetInvitationByIdAsync(invitationId, cancellationToken);
            if (invitation == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResult("Invitation not found"));
            }

            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, invitation.VenueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            // Cancel the invitation
            var success = await _permissionService.RevokeInvitationAsync(invitationId, cancellationToken);
            
            if (!success)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Failed to cancel invitation"));
            }

            _logger.LogInformation("Invitation {InvitationId} cancelled by user {UserSub}", invitationId, userSub);
            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling invitation {InvitationId} by user {UserSub}", invitationId, userSub);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get valid permission types for venue management
    /// </summary>
    [HttpGet("permission-types")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PermissionTypeResponse>>>> GetPermissionTypes(CancellationToken cancellationToken = default)
    {
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        try
        {
            var permissionTypes = await _permissionTypeService.GetValidPermissionTypesAsync(cancellationToken);
            
            var result = new List<PermissionTypeResponse>();
            foreach (var permissionType in permissionTypes)
            {
                var displayName = await _permissionTypeService.GetPermissionDisplayNameAsync(permissionType, cancellationToken);
                var hierarchy = await _permissionTypeService.GetPermissionHierarchyAsync(permissionType, cancellationToken);
                
                result.Add(new PermissionTypeResponse
                {
                    Value = permissionType,
                    DisplayName = displayName,
                    Hierarchy = hierarchy
                });
            }

            return Ok(ApiResponse<IEnumerable<PermissionTypeResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission types");
            return StatusCode(500, ApiResponse<IEnumerable<PermissionTypeResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Update/sync user information from frontend token
    /// </summary>
    [HttpPost("sync-user")]
    public async Task<ActionResult<ApiResponse<UserInfoResponse>>> SyncUser(
        [FromBody] UserInfo userInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserInfoResponse>.ErrorResult("Invalid user information"));
            }

            // Ensure user exists with the provided information
            var userId = await _permissionService.EnsureUserExistsAsync(
                userInfo.Sub, 
                userInfo.Email, 
                userInfo.Name, 
                cancellationToken);

            // Get the updated user to return
            var user = await _permissionService.GetUserBySubAsync(userInfo.Sub, cancellationToken);
            if (user == null)
            {
                return StatusCode(500, ApiResponse<UserInfoResponse>.ErrorResult("Failed to retrieve user after sync"));
            }

            var response = new UserInfoResponse
            {
                Id = user.Id,
                Sub = user.Sub,
                Email = user.Email,
                Name = null, // UserEntity doesn't have Name property yet
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt.ToDateTimeUtc(),
                UpdatedAt = user.UpdatedAt.ToDateTimeUtc(),
                LastLoginAt = user.LastLoginAt?.ToDateTimeUtc()
            };

            return Ok(ApiResponse<UserInfoResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user information for {UserSub}", userInfo.Sub);
            return StatusCode(500, ApiResponse<UserInfoResponse>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Update user permission for a venue
    /// </summary>
    [HttpPut("permissions/{permissionId}")]
    public async Task<ActionResult<ApiResponse<VenuePermissionResponse>>> UpdateUserPermission(
        long permissionId,
        [FromBody] UpdatePermissionRequest request,
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
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VenuePermissionResponse>.ErrorResult("Invalid permission data"));
            }

            // Update the permission
            var updatedPermission = await _permissionService.UpdateUserVenuePermissionAsync(
                permissionId, request.Permission, userSub, request.Notes, cancellationToken);

            if (updatedPermission == null)
            {
                return NotFound(ApiResponse<VenuePermissionResponse>.ErrorResult("Permission not found"));
            }

            // Create response
            var response = new VenuePermissionResponse
            {
                Id = updatedPermission.Id,
                UserId = updatedPermission.UserId,
                VenueId = updatedPermission.VenueId,
                VenueName = updatedPermission.Venue?.Name ?? "",
                Name = updatedPermission.Name,
                UserEmail = updatedPermission.User?.Email ?? "",
                GrantedByUserId = updatedPermission.GrantedByUserId,
                GrantedByUserEmail = updatedPermission.GrantedByUser?.Email ?? "",
                GrantedAt = updatedPermission.GrantedAt.ToDateTimeUtc(),
                IsActive = updatedPermission.IsActive,
                Notes = updatedPermission.Notes
            };

            return Ok(ApiResponse<VenuePermissionResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission {PermissionId}", permissionId);
            return StatusCode(500, ApiResponse<VenuePermissionResponse>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Revoke user permission for a venue
    /// </summary>
    [HttpDelete("permissions/{permissionId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeUserPermission(
        long permissionId,
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
            // Revoke the permission
            var success = await _permissionService.RevokeUserVenuePermissionAsync(permissionId, userSub, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResult("Permission not found or could not be revoked"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission {PermissionId}", permissionId);
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }
}
