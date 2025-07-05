namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for venue permission types
/// </summary>
public class PermissionTypeResponse
{
    public required string Value { get; set; }
    public required string DisplayName { get; set; }
    public string[] Hierarchy { get; set; } = Array.Empty<string>();
}
