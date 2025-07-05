namespace Application.Common.Models.Location;

/// <summary>
/// Result of geocoding an address to coordinates
/// </summary>
public class GeocodeResult
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string FormattedAddress { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public double Confidence { get; set; }
}
