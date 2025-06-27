namespace Application.Common.Models.Location;

/// <summary>
/// Result of reverse geocoding coordinates to address
/// </summary>
public class ReverseGeocodeResult
{
    public string FormattedAddress { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
}
