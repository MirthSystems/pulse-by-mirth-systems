using Microsoft.AspNetCore.Authorization;

namespace Application.Infrastructure.Authorization.Requirements;

/// <summary>
/// Authorization requirement for backoffice access
/// Requires either Auth0 system permissions OR venue-specific permissions
/// </summary>
public class BackofficeAccessRequirement : IAuthorizationRequirement 
{
}
