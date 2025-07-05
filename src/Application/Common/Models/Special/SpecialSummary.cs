namespace Application.Common.Models.Special;

/// <summary>
/// Simplified special model for lists and search results
/// </summary>
public class SpecialSummary : SpecialBase
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public required string VenueName { get; set; }
    public int SpecialCategoryId { get; set; }
    public required string CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
    public double? DistanceInMeters { get; set; }
}
