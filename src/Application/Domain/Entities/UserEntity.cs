using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace Application.Domain.Entities;

/// <summary>
/// Represents a user in the system with Auth0 integration
/// </summary>
[Table("users")]
public class UserEntity
{
    [Column("id")]
    public long Id { get; set; }

    [Column("sub")]
    [Required]
    [MaxLength(100)]
    public string Sub { get; set; } = null!; // Auth0 'sub' claim

    [Column("email")]
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public Instant CreatedAt { get; set; }

    [Column("updated_at")]
    public Instant UpdatedAt { get; set; }

    [Column("last_login_at")]
    public Instant? LastLoginAt { get; set; }

    // Navigation properties
    public List<UserVenuePermissionEntity> VenuePermissions { get; set; } = new List<UserVenuePermissionEntity>();
    public List<VenueInvitationEntity> SentInvitations { get; set; } = new List<VenueInvitationEntity>();
    public List<VenueInvitationEntity> ReceivedInvitations { get; set; } = new List<VenueInvitationEntity>();
}
