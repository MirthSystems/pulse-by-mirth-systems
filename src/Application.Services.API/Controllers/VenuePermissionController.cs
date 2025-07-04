using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Common.Utilities;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemClaimTypes = System.Security.Claims.ClaimTypes;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for venue permission and invitation management
/// Handles permissions, invitations, and access control for venues
/// </summary>
[ApiController]
[Authorize]
public class VenuePermissionController : BaseApiController
{
    private readonly IPermissionService _permissionService;
    private readonly IVenuePermissionTypeService _permissionTypeService;
    private readonly IConfiguration _configuration;

    public VenuePermissionController(
        IPermissionService permissionService,
        IVenuePermissionTypeService permissionTypeService,
        IConfiguration configuration,
        IAuthorizationService authorizationService,
        ILogger<VenuePermissionController> logger)
        : base(logger, authorizationService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _permissionTypeService = permissionTypeService ?? throw new ArgumentNullException(nameof(permissionTypeService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Get current user's venue permissions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's venue permissions</returns>
    [HttpGet(ApiRoutes.Permissions.GetMine)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenuePermissionSummary>>>> GetMyPermissions(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetMyPermissions));
        
        try
        {
            var userSub = UserContextUtils.GetUserSub(User);
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(ApiResponse<IEnumerable<VenuePermissionSummary>>.ErrorResult("User not authenticated"));
            }

            var permissions = await _permissionService.GetUserVenuePermissionsAsync(userSub, cancellationToken);
            
            LogActionComplete(nameof(GetMyPermissions), true);
            return Ok(ApiResponse<IEnumerable<VenuePermissionSummary>>.SuccessResult(permissions));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMyPermissions));
            return InternalServerError<IEnumerable<VenuePermissionSummary>>();
        }
    }

    /// <summary>
    /// Send an invitation to a user for venue access
    /// </summary>
    /// <param name="request">Invitation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created invitation</returns>
    [HttpPost(ApiRoutes.Invitations.Create)]
    public async Task<ActionResult<ApiResponse<VenueInvitationResponse>>> SendInvitation(
        [FromBody] CreateInvitationRequest request,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SendInvitation), request);
        
        try
        {
            var userSub = UserContextUtils.GetUserSub(User);
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(ApiResponse<VenueInvitationResponse>.ErrorResult("User not authenticated"));
            }

            // Validate request
            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            // Check if user has system permissions or venue access
            if (!PermissionUtils.HasSystemPermissions(User))
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub, request.VenueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var invitation = await _permissionService.CreateInvitationAsync(request, userSub, request.SenderEmail, cancellationToken);
            if (invitation == null)
            {
                return BadRequest(ApiResponse<VenueInvitationResponse>.ErrorResult("Failed to create invitation"));
            }

            // Convert to simplified response (focusing on available properties)
            var response = new VenueInvitationResponse
            {
                Id = invitation.Id,
                Email = invitation.Email,
                VenueId = invitation.VenueId,
                VenueName = invitation.Venue?.Name ?? "",
                InvitedByUserId = invitation.InvitedByUserId,
                InvitedByUserEmail = invitation.InvitedByUser?.Email ?? "",
                InvitedAt = invitation.InvitedAt.ToDateTimeUtc(),
                ExpiresAt = invitation.ExpiresAt.ToDateTimeUtc(),
                IsActive = invitation.IsActive,
                Notes = invitation.Notes
            };

            LogActionComplete(nameof(SendInvitation), true);
            return Ok(ApiResponse<VenueInvitationResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SendInvitation), request);
            return InternalServerError<VenueInvitationResponse>();
        }
    }

    /// <summary>
    /// Get current user's pending invitations
    /// </summary>
    /// <param name="email">Optional email parameter to override JWT claim lookup</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's pending invitations</returns>
    [HttpGet(ApiRoutes.Invitations.GetMine)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueInvitationResponse>>>> GetMyInvitations(
        [FromQuery] string? email = null,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetMyInvitations));
        
        try
        {
            string? userEmail = null;
            
            // If email is provided as query parameter, use it (for frontend compatibility)
            if (!string.IsNullOrWhiteSpace(email))
            {
                userEmail = email;
                _logger.LogInformation("Using email from query parameter: {Email}", email);
            }
            else
            {
                // Get audience from configuration for comprehensive email claim lookup
                var audience = _configuration["Auth0:Audience"];
                userEmail = UserContextUtils.GetUserEmail(User, audience);
            }
            
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email not found in query parameter or claims. Available claims: {Claims}", 
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                return Unauthorized(ApiResponse<IEnumerable<VenueInvitationResponse>>.ErrorResult("User email not found"));
            }

            var invitations = await _permissionService.GetUserPendingInvitationsAsync(userEmail, cancellationToken);
            var response = invitations.Select(i => new VenueInvitationResponse
            {
                Id = i.Id,
                Email = i.Email,
                VenueId = i.VenueId,
                VenueName = i.Venue?.Name ?? "",
                InvitedByUserId = i.InvitedByUserId,
                InvitedByUserEmail = i.InvitedByUser?.Email ?? "",
                InvitedAt = i.InvitedAt.ToDateTimeUtc(),
                ExpiresAt = i.ExpiresAt.ToDateTimeUtc(),
                IsActive = i.IsActive,
                Notes = i.Notes
            });
            
            LogActionComplete(nameof(GetMyInvitations), true);
            return Ok(ApiResponse<IEnumerable<VenueInvitationResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMyInvitations));
            return InternalServerError<IEnumerable<VenueInvitationResponse>>();
        }
    }

    /// <summary>
    /// Get available permission types (placeholder implementation)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available permission types</returns>
    [HttpGet(ApiRoutes.Permissions.Types)]
    public Task<ActionResult<ApiResponse<IEnumerable<PermissionTypeResponse>>>> GetPermissionTypes(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetPermissionTypes));
        
        try
        {
            // Placeholder implementation - return empty list for now
            var permissionTypes = new List<PermissionTypeResponse>();
            
            LogActionComplete(nameof(GetPermissionTypes), true);
            return Task.FromResult<ActionResult<ApiResponse<IEnumerable<PermissionTypeResponse>>>>(
                Ok(ApiResponse<IEnumerable<PermissionTypeResponse>>.SuccessResult(permissionTypes)));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetPermissionTypes));
            return Task.FromResult<ActionResult<ApiResponse<IEnumerable<PermissionTypeResponse>>>>(
                InternalServerError<IEnumerable<PermissionTypeResponse>>());
        }
    }

    /// <summary>
    /// Update a user's permission for a venue (placeholder implementation)
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated permission</returns>
    [HttpPut(ApiRoutes.Permissions.Update)]
    public Task<ActionResult<ApiResponse<VenuePermissionResponse>>> UpdateUserPermission(
        long permissionId,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(UpdateUserPermission), new { permissionId, request });
        
        // Placeholder implementation - to be completed when service methods are available
        LogActionComplete(nameof(UpdateUserPermission), false);
        return Task.FromResult<ActionResult<ApiResponse<VenuePermissionResponse>>>(
            BadRequest(ApiResponse<VenuePermissionResponse>.ErrorResult("Update permission functionality not yet implemented")));
    }

    /// <summary>
    /// Revoke a user's permission for a venue (placeholder implementation)
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpDelete(ApiRoutes.Permissions.Revoke)]
    public Task<ActionResult<ApiResponse<bool>>> RevokeUserPermission(
        long permissionId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(RevokeUserPermission), new { permissionId });
        
        // Placeholder implementation - to be completed when service methods are available
        LogActionComplete(nameof(RevokeUserPermission), false);
        return Task.FromResult<ActionResult<ApiResponse<bool>>>(
            BadRequest(ApiResponse<bool>.ErrorResult("Revoke permission functionality not yet implemented")));
    }

    /// <summary>
    /// Get permissions for a specific venue
    /// </summary>
    /// <param name="venueId">Venue ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of venue permissions</returns>
    [HttpGet(ApiRoutes.Permissions.GetByVenue)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenuePermissionResponse>>>> GetVenuePermissions(
        long venueId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenuePermissions), new { venueId });
        
        var (isAuthorized, userSub, errorResult) = await ValidateBackofficeAccessAsync();
        if (!isAuthorized)
        {
            return errorResult!;
        }

        try
        {
            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub!, venueId, cancellationToken);
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

            LogActionComplete(nameof(GetVenuePermissions), true);
            return Ok(ApiResponse<IEnumerable<VenuePermissionResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenuePermissions), new { venueId });
            return StatusCode(500, ApiResponse<IEnumerable<VenuePermissionResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Get invitations for a specific venue
    /// </summary>
    /// <param name="venueId">Venue ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of venue invitations</returns>
    [HttpGet(ApiRoutes.Invitations.GetByVenue)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueInvitationResponse>>>> GetVenueInvitations(
        long venueId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetVenueInvitations), new { venueId });
        
        var (isAuthorized, userSub, errorResult) = await ValidateBackofficeAccessAsync();
        if (!isAuthorized)
        {
            return errorResult!;
        }

        try
        {
            // Check if user can manage this venue (unless they're admin/content manager)
            var isSystemAdmin = User.HasClaim("permissions", "system:admin");
            var isContentManager = User.HasClaim("permissions", "content:manager");
            
            if (!isSystemAdmin && !isContentManager)
            {
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub!, venueId, cancellationToken);
                if (!hasVenueAccess)
                {
                    return Forbid();
                }
            }

            var invitations = await _permissionService.GetVenueInvitationsAsync(venueId, cancellationToken);
            
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

            LogActionComplete(nameof(GetVenueInvitations), true);
            return Ok(ApiResponse<IEnumerable<VenueInvitationResponse>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetVenueInvitations), new { venueId });
            return StatusCode(500, ApiResponse<IEnumerable<VenueInvitationResponse>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Accept an invitation
    /// </summary>
    /// <param name="invitationId">Invitation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost(ApiRoutes.Invitations.Accept)]
    public async Task<ActionResult<ApiResponse<bool>>> AcceptInvitation(
        long invitationId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(AcceptInvitation), new { invitationId });
        
        var userSub = UserContextUtils.GetUserSub(User);
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Use centralized email claim extraction with audience support
            var audience = _configuration["Auth0:Audience"];
            var userEmail = UserContextUtils.GetUserEmail(User, audience);
            
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

            LogActionComplete(nameof(AcceptInvitation), true);
            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(AcceptInvitation), new { invitationId });
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Decline an invitation
    /// </summary>
    /// <param name="invitationId">Invitation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost(ApiRoutes.Invitations.Decline)]
    public async Task<ActionResult<ApiResponse<bool>>> DeclineInvitation(
        long invitationId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(DeclineInvitation), new { invitationId });
        
        var userSub = UserContextUtils.GetUserSub(User);
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            // Use centralized email claim extraction with audience support
            var audience = _configuration["Auth0:Audience"];
            var userEmail = UserContextUtils.GetUserEmail(User, audience);
            
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

            LogActionComplete(nameof(DeclineInvitation), true);
            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(DeclineInvitation), new { invitationId });
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Cancel/revoke an invitation (for venue owners/managers)
    /// </summary>
    /// <param name="invitationId">Invitation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpDelete(ApiRoutes.Invitations.Cancel)]
    public async Task<ActionResult<ApiResponse<bool>>> CancelInvitation(
        long invitationId,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(CancelInvitation), new { invitationId });
        
        var (isAuthorized, userSub, errorResult) = await ValidateBackofficeAccessAsync();
        if (!isAuthorized)
        {
            return errorResult!;
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
                var hasVenueAccess = await _permissionService.HasVenuePermissionAsync(userSub!, invitation.VenueId, cancellationToken);
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

            LogActionComplete(nameof(CancelInvitation), true);
            return Ok(ApiResponse<bool>.SuccessResult(true));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(CancelInvitation), new { invitationId });
            return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
        }
    }
}
