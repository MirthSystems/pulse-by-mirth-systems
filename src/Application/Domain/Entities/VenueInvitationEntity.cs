using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace Application.Domain.Entities;

/// <summary>
/// Represents invitations for users to join venue management
/// </summary>
[Table("venue_invitations")]
public class VenueInvitationEntity
{
    [Column("id")]
    public long Id { get; set; }

    [Column("email")]
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;

    [Column("venue_id")]
    [Required]
    public long VenueId { get; set; }

    [Column("permission")]
    [Required]
    [MaxLength(20)]
    public string Permission { get; set; } = null!; // "venue:owner" | "venue:manager" | "venue:staff" 

    [Column("invited_by_user_id")]
    [Required]
    public long InvitedByUserId { get; set; }

    [Column("invited_at")]
    public Instant InvitedAt { get; set; }

    [Column("expires_at")]
    public Instant ExpiresAt { get; set; }

    [Column("accepted_at")]
    public Instant? AcceptedAt { get; set; }

    [Column("accepted_by_user_id")]
    public long? AcceptedByUserId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public VenueEntity Venue { get; set; } = null!;
    public UserEntity InvitedByUser { get; set; } = null!;
    public UserEntity? AcceptedByUser { get; set; }

    /// <summary>
    /// Checks if the invitation is still valid (not expired and active)
    /// </summary>
    public bool IsValid => IsActive && !AcceptedAt.HasValue && ExpiresAt > SystemClock.Instance.GetCurrentInstant();

    /// <summary>
    /// Checks if the invitation has been accepted
    /// </summary>
    public bool IsAccepted => AcceptedAt.HasValue && AcceptedByUserId.HasValue;
}
