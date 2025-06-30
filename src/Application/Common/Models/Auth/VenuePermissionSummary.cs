namespace Application.Common.Models.Auth;

/// <summary>
/// Summary of user's venue permissions
/// </summary>
public class VenuePermissionSummary
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long VenueId { get; set; }
    public string VenueName { get; set; } = null!;
    public string Permission { get; set; } = null!;
    public long GrantedByUserId { get; set; }
    public string GrantedByName { get; set; } = null!;
    public DateTime GrantedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
