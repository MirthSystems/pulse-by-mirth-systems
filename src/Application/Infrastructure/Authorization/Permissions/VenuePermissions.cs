namespace Application.Infrastructure.Authorization.Permissions;

/// <summary>
/// Constants for venue permission roles
/// </summary>
public static class VenuePermissions
{
    public const string Owner = "venue:owner";
    public const string Manager = "venue:manager";
    public const string Staff = "venue:staff";

    public static readonly string[] AllPermissions = { Owner, Manager, Staff };

    public static bool IsValidPermission(string permission) => AllPermissions.Contains(permission);
}
