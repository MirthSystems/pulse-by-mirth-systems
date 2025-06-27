namespace Application.Common.Models.Venue;

/// <summary>
/// Business hours model
/// </summary>
public class BusinessHours
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public byte DayOfWeekId { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string DayOfWeekShortName { get; set; } = string.Empty;
    public string? OpenTime { get; set; }  // Format: "HH:mm"
    public string? CloseTime { get; set; } // Format: "HH:mm"
    public bool IsClosed { get; set; }
    public int SortOrder { get; set; }
}
