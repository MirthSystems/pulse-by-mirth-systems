namespace Application.Common.Models.Special;

/// <summary>
/// Base special model with common properties
/// </summary>
public abstract class SpecialBase
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StartDate { get; set; } = string.Empty;  // Format: "yyyy-MM-dd"
    public string StartTime { get; set; } = string.Empty;  // Format: "HH:mm"
    public string? EndTime { get; set; }                   // Format: "HH:mm"
    public string? EndDate { get; set; }                   // Format: "yyyy-MM-dd"
    public bool IsRecurring { get; set; }
    public string? CronSchedule { get; set; }
    public bool IsActive { get; set; }
}
