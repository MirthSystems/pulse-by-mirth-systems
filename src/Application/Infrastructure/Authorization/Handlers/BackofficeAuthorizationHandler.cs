using Application.Common.Interfaces.Services;
using Application.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Application.Infrastructure.Authorization.Handlers;

/// <summary>
/// Authorization handler for backoffice access requirements
/// Allows access if user has Auth0 permissions OR any venue permissions
/// </summary>
public class BackofficeAuthorizationHandler : AuthorizationHandler<BackofficeAccessRequirement>
{
    private readonly IPermissionService _permissionService;

    public BackofficeAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BackofficeAccessRequirement requirement)
    {
        var userSub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            context.Fail();
            return;
        }

        // System admin has full backoffice access
        if (context.User.HasClaim("permissions", "system:admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Content manager has full backoffice access
        if (context.User.HasClaim("permissions", "content:manager"))
        {
            context.Succeed(requirement);
            return;
        }

        // Check if user has ANY venue permissions (venue owner/manager/staff)
        var userVenueIds = await _permissionService.GetUserVenueIdsAsync(userSub);
        if (userVenueIds.Any())
        {
            context.Succeed(requirement);
            return;
        }

        // No backoffice access found
        context.Fail();
    }
}
