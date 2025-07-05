namespace Application.Common.Models.Auth;

/// <summary>
/// Summary of user's venue permissions
/// </summary>
public class VenuePermissionSummary
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long VenueId { get; set; }
    public required string VenueName { get; set; }
    public required string Name { get; set; }
    public long GrantedByUserId { get; set; }
    public required string GrantedByName { get; set; }
    public DateTime GrantedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
