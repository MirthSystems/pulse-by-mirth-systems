namespace Application.Common.Models.Location;

/// <summary>
/// Point of Interest found during search
/// </summary>
public class PointOfInterest
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string Website { get; set; }
    public double DistanceInMeters { get; set; }
    public double Score { get; set; }
}
