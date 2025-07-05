namespace Application.Common.Constants;

/// <summary>
/// Authorization policy constants
/// </summary>
public static class AuthorizationPolicies
{
    public const string RequireAuthentication = "RequireAuthentication";
    public const string RequireBackofficeAccess = "RequireBackofficeAccess";
    public const string RequireSystemAdmin = "RequireSystemAdmin";
    public const string RequireContentManager = "RequireContentManager";
    public const string RequireVenueOwner = "RequireVenueOwner";
    public const string RequireVenueManager = "RequireVenueManager";
    public const string RequireVenueStaff = "RequireVenueStaff";
}

/// <summary>
/// Auth0 permission constants
/// </summary>
public static class Auth0Permissions
{
    public const string SystemAdmin = "system:admin";
    public const string ContentManager = "content:manager";
    public const string VenueOwner = "venue:owner";
    public const string VenueManager = "venue:manager";
    public const string VenueStaff = "venue:staff";
}

/// <summary>
/// Claims type constants
/// </summary>
public static class ClaimTypes
{
    public const string UserId = "user_id";
    public const string UserSub = "sub";
    public const string Email = "email";
    public const string Name = "name";
    public const string Permissions = "permissions";
    public const string Roles = "roles";
}
