namespace Application.Common.Models.Location;

/// <summary>
/// Point of Interest found during search
/// </summary>
public class PointOfInterest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public double DistanceInMeters { get; set; }
    public double Score { get; set; }
}
