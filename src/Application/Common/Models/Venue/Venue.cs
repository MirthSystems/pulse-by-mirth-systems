namespace Application.Common.Models.Venue;

/// <summary>
/// Full venue model for responses
/// </summary>
public class Venue : VenueBase
{
    public long Id { get; set; }
    public int CategoryId { get; set; }
    public VenueCategory? Category { get; set; }
    public List<BusinessHours> BusinessHours { get; set; } = new();
    public List<Application.Common.Models.Special.SpecialSummary> ActiveSpecials { get; set; } = new();
    public double? DistanceInMeters { get; set; }
}
