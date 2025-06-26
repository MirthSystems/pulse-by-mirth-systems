namespace Application.Common.Models.Location;

/// <summary>
/// Location search request parameters
/// </summary>
public class LocationSearch
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Category { get; set; }
    public int RadiusInMeters { get; set; } = 5000;
    public int MaxResults { get; set; } = 50;
    public string? SearchTerm { get; set; }
}
