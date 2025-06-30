namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for venue permissions
/// </summary>
public class VenuePermissionResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long VenueId { get; set; }
    public string VenueName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public long GrantedByUserId { get; set; }
    public string GrantedByUserEmail { get; set; } = null!;
    public DateTime GrantedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
