namespace Application.Common.Models.Auth;

/// <summary>
/// Summary of user's venue permissions
/// </summary>
public class VenuePermissionSummary
{
    public long VenueId { get; set; }
    public string VenueName { get; set; } = null!;
    public string Permission { get; set; } = null!;
    public DateTime GrantedAt { get; set; }
    public string GrantedByName { get; set; } = null!;
    public bool IsActive { get; set; }
}
