namespace Application.Common.Models.Special;

/// <summary>
/// Simplified special model for lists and search results
/// </summary>
public class SpecialSummary : SpecialBase
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public int SpecialCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryIcon { get; set; }
    public double? DistanceInMeters { get; set; }
}
