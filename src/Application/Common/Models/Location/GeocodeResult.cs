namespace Application.Common.Models.Location;

/// <summary>
/// Result of geocoding an address to coordinates
/// </summary>
public class GeocodeResult
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string FormattedAddress { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Confidence { get; set; }
}
