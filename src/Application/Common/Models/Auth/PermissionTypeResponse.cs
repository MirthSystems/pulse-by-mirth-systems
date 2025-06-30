namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for venue permission types
/// </summary>
public class PermissionTypeResponse
{
    public string Value { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string[] Hierarchy { get; set; } = Array.Empty<string>();
}
