using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace Application.Domain.Entities;

/// <summary>
/// Represents venue-specific permissions for users
/// </summary>
[Table("user_venue_permissions")]
public class UserVenuePermissionEntity
{
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    [Required]
    public long UserId { get; set; }

    [Column("venue_id")]
    [Required]
    public long VenueId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!; // "venue:owner" | "venue:manager" | "venue:staff" 

    [Column("granted_by_user_id")]
    [Required]
    public long GrantedByUserId { get; set; }

    [Column("granted_at")]
    public Instant GrantedAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public UserEntity User { get; set; } = null!;
    public VenueEntity Venue { get; set; } = null!;
    public UserEntity GrantedByUser { get; set; } = null!;
}
