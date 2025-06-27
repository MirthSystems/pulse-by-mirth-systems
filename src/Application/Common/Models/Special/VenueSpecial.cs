namespace Application.Common.Models.Special;

/// <summary>
/// Simplified venue model for use in special responses
/// </summary>
public class VenueSpecial
{
    public long Id { get; set; }
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
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryIcon { get; set; }
    public double? DistanceInMeters { get; set; }
}
