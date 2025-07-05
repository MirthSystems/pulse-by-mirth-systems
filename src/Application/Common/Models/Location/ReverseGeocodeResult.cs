namespace Application.Common.Models.Location;

/// <summary>
/// Result of reverse geocoding coordinates to address
/// </summary>
public class ReverseGeocodeResult
{
    public required string FormattedAddress { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public required string Neighborhood { get; set; }
}
