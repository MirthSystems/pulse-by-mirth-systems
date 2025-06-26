using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Venue;

/// <summary>
/// Base venue model with common properties
/// </summary>
public abstract class VenueBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public string? SecondaryAddress { get; set; }
    public string Locality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsActive { get; set; }
}
