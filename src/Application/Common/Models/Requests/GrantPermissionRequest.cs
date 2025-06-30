namespace Application.Common.Models.Requests;

/// <summary>
/// Request model for granting permission
/// </summary>
public class GrantPermissionRequest
{
    public string UserSub { get; set; } = null!;
    public string Permission { get; set; } = null!;
    public string? Notes { get; set; }
}
