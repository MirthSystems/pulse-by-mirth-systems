using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Venue;

/// <summary>
/// Base venue model with common properties
/// </summary>
public abstract class VenueBase
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [Url]
    [MaxLength(200)]
    public string? Website { get; set; }
    
    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [Url]
    [MaxLength(500)]
    public string? ProfileImage { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string StreetAddress { get; set; }
    
    [MaxLength(100)]
    public string? SecondaryAddress { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Locality { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Region { get; set; }
    
    [Required]
    [MaxLength(20)]
    public required string PostalCode { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Country { get; set; }
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsActive { get; set; }
}
