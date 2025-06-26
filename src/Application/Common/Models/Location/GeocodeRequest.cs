namespace Application.Common.Models.Location;

/// <summary>
/// Address geocoding request
/// </summary>
public class GeocodeRequest
{
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}
