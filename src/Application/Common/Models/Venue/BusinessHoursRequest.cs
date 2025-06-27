namespace Application.Common.Models.Venue;

/// <summary>
/// Business hours request model for creating/updating venue business hours
/// </summary>
public class BusinessHoursRequest
{
    public byte DayOfWeekId { get; set; }
    public string? OpenTime { get; set; }  // Format: "HH:mm"
    public string? CloseTime { get; set; } // Format: "HH:mm"
    public bool IsClosed { get; set; }
}
