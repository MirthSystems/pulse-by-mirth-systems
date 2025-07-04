using Application.Common.Constants;
using Application.Common.Models;
using Application.Common.Utilities;
using Application.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SystemClaimTypes = System.Security.Claims.ClaimTypes;
using SystemClaimsPrincipal = System.Security.Claims.ClaimsPrincipal;

namespace Application.Services.API.Controllers.Base;

/// <summary>
/// Base controller providing common functionality for all API controllers
/// Implements consistent patterns for authorization, validation, logging, and error handling
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;
    protected readonly IAuthorizationService? _authorizationService;

    protected BaseApiController(ILogger logger, IAuthorizationService? authorizationService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Validates model state and returns appropriate error response if invalid
    /// </summary>
    protected ActionResult? ValidateModelState()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            _logger.LogWarning("Model validation failed: {Errors}", 
                System.Text.Json.JsonSerializer.Serialize(errors));

            return BadRequest(ApiResponse<object>.ErrorResult("Invalid request data", errors));
        }

        return null;
    }

    /// <summary>
    /// Validates backoffice access and returns user context
    /// </summary>
    protected async Task<(bool isAuthorized, string? userSub, ActionResult? errorResult)> ValidateBackofficeAccessAsync()
    {
        if (_authorizationService == null)
        {
            _logger.LogError("Authorization service not available");
            return (false, null, StatusCode(HttpStatusConstants.InternalServerError, 
                ApiResponse<object>.ErrorResult(ErrorMessages.InternalServerError)));
        }

        // Check user authentication first
        var userSub = User.FindFirst(SystemClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            _logger.LogWarning("Unauthorized access attempt - no user identifier found");
            return (false, null, Unauthorized());
        }
        
        // Check backoffice access
        var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
        if (!authResult.Succeeded)
        {
            _logger.LogWarning("Forbidden access attempt by user {UserSub}", userSub);
            return (false, userSub, Forbid());
        }
        
        return (true, userSub, null);
    }

    /// <summary>
    /// Handles service response and returns appropriate ActionResult
    /// </summary>
    protected ActionResult<T> HandleServiceResponse<T>(ApiResponse<T> response)
    {
        if (response.Success)
        {
            return Ok(response);
        }

        // Log the error details
        _logger.LogWarning("Service returned error response: {Message}", response.Message);

        // Return appropriate status code based on error
        if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
        {
            return NotFound(response);
        }

        if (response.Message?.Contains("forbidden", StringComparison.OrdinalIgnoreCase) == true ||
            response.Message?.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Forbid();
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Logs the start of a controller action with relevant context
    /// </summary>
    protected void LogActionStart(string actionName, object? parameters = null)
    {
        var userSub = UserContextHelper.GetUserSub(User);
        var paramInfo = parameters != null ? $" with parameters: {System.Text.Json.JsonSerializer.Serialize(parameters)}" : "";
        
        _logger.LogInformation("Starting {ActionName} for user {UserSub}{ParameterInfo}", 
            actionName, userSub, paramInfo);
    }

    /// <summary>
    /// Logs the completion of a controller action
    /// </summary>
    protected void LogActionComplete(string actionName, bool success, object? result = null)
    {
        var userSub = UserContextHelper.GetUserSub(User);
        var status = success ? "completed successfully" : "failed";
        
        _logger.LogInformation("Action {ActionName} for user {UserSub} {Status}", 
            actionName, userSub, status);
    }

    /// <summary>
    /// Logs an error with standardized context information
    /// </summary>
    protected void LogError(Exception ex, string actionName, object? context = null)
    {
        var userSub = UserContextHelper.GetUserSub(User);
        var contextInfo = context != null ? System.Text.Json.JsonSerializer.Serialize(context) : "No additional context";
        
        _logger.LogError(ex, "Error in {ActionName} for user {UserSub}. Context: {Context}", 
            actionName, userSub, contextInfo);
    }

    /// <summary>
    /// Creates a standardized internal server error response
    /// </summary>
    protected ActionResult InternalServerError<T>()
    {
        return StatusCode(HttpStatusConstants.InternalServerError, 
            ApiResponse<T>.ErrorResult(ErrorMessages.InternalServerError));
    }
}