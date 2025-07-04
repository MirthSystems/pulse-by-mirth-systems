using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Common.Utilities;
using Application.Infrastructure.Authorization.Permissions;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemClaimTypes = System.Security.Claims.ClaimTypes;

namespace Application.Services.API.Controllers;

/// <summary>
/// API controller for user management operations
/// Handles user synchronization and profile management
/// </summary>
[ApiController]
[Authorize]
public class UserManagementController : BaseApiController
{
    private readonly IPermissionService _permissionService;
    private readonly IConfiguration _configuration;

    public UserManagementController(
        IPermissionService permissionService,
        IConfiguration configuration,
        ILogger<UserManagementController> logger)
        : base(logger)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Synchronize user data from Auth0 to local database
    /// Creates or updates user record based on Auth0 profile
    /// </summary>
    /// <param name="request">User info from Auth0</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information response</returns>
    [HttpPost(ApiRoutes.Users.Sync)]
    public async Task<ActionResult<ApiResponse<UserInfoResponse>>> SyncUser(
        [FromBody] UserInfo request,
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(SyncUser), request);
        
        try
        {
            // Validate request
            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            // Ensure user exists with the provided information
            var userId = await _permissionService.EnsureUserExistsAsync(
                request.Sub, 
                request.Email, 
                request.Name, 
                cancellationToken);

            // Get the updated user to return
            var user = await _permissionService.GetUserBySubAsync(request.Sub, cancellationToken);
            if (user == null)
            {
                LogActionComplete(nameof(SyncUser), false);
                return BadRequest(ApiResponse<UserInfoResponse>.ErrorResult("Failed to retrieve user after sync"));
            }

            var response = new UserInfoResponse
            {
                Id = user.Id,
                Sub = user.Sub,
                Email = user.Email,
                Name = request.Name, 
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt.ToDateTimeUtc(),
                UpdatedAt = user.UpdatedAt.ToDateTimeUtc(),
                LastLoginAt = user.LastLoginAt?.ToDateTimeUtc()
            };

            LogActionComplete(nameof(SyncUser), true);
            return Ok(ApiResponse<UserInfoResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(SyncUser), request);
            return InternalServerError<UserInfoResponse>();
        }
    }

    /// <summary>
    /// Get current user's profile information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    [HttpGet(ApiRoutes.Users.Profile)]
    public async Task<ActionResult<ApiResponse<UserInfoResponse>>> GetProfile(CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetProfile));
        
        try
        {
            var userSub = UserContextHelper.GetUserSub(User);
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(ApiResponse<UserInfoResponse>.ErrorResult("User not authenticated"));
            }

            var user = await _permissionService.GetUserBySubAsync(userSub, cancellationToken);
            if (user == null)
            {
                LogActionComplete(nameof(GetProfile), false);
                return NotFound(ApiResponse<UserInfoResponse>.ErrorResult("User not found"));
            }

            var response = new UserInfoResponse
            {
                Id = user.Id,
                Sub = user.Sub,
                Email = user.Email,
                Name = User.FindFirst(SystemClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt.ToDateTimeUtc(),
                UpdatedAt = user.UpdatedAt.ToDateTimeUtc(),
                LastLoginAt = user.LastLoginAt?.ToDateTimeUtc()
            };

            LogActionComplete(nameof(GetProfile), true);
            return Ok(ApiResponse<UserInfoResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetProfile));
            return InternalServerError<UserInfoResponse>();
        }
    }
}
