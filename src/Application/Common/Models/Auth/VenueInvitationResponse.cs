namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for venue invitations
/// </summary>
public class VenueInvitationResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public long VenueId { get; set; }
    public string VenueName { get; set; } = null!;
    public string Permission { get; set; } = null!;
    public long InvitedByUserId { get; set; }
    public string InvitedByUserEmail { get; set; } = null!;
    public DateTime InvitedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public long? AcceptedByUserId { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
