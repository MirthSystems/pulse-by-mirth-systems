namespace Application.Common.Models.Special;

/// <summary>
/// Simplified venue model for use in special responses
/// </summary>
public class VenueSpecial
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public required string StreetAddress { get; set; }
    public string? SecondaryAddress { get; set; }
    public required string Locality { get; set; }
    public required string Region { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
    public double? DistanceInMeters { get; set; }
}
