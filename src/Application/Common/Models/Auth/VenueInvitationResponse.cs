namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for venue invitations
/// </summary>
public class VenueInvitationResponse
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public long VenueId { get; set; }
    public required string VenueName { get; set; }
    public required string Permission { get; set; }
    public long InvitedByUserId { get; set; }
    public required string InvitedByUserEmail { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public long? AcceptedByUserId { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
