namespace Application.Common.Models.Venue;

/// <summary>
/// Simplified venue model for lists and search results
/// </summary>
public class VenueSummary : VenueBase
{
    public long Id { get; set; }
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
    public int ActiveSpecialsCount { get; set; }
    public double? DistanceInMeters { get; set; }
}
