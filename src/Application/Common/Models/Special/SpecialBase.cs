using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Special;

/// <summary>
/// Base special model with common properties
/// </summary>
public abstract class SpecialBase
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "StartDate must be in yyyy-MM-dd format")]
    public required string StartDate { get; set; }  // Format: "yyyy-MM-dd"
    
    [Required]
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "StartTime must be in HH:mm format")]
    public required string StartTime { get; set; }  // Format: "HH:mm"
    
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "EndTime must be in HH:mm format")]
    public string? EndTime { get; set; }                   // Format: "HH:mm"
    
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "EndDate must be in yyyy-MM-dd format")]
    public string? EndDate { get; set; }                   // Format: "yyyy-MM-dd"
    
    public bool IsRecurring { get; set; }
    
    [MaxLength(100)]
    public string? CronSchedule { get; set; }
    
    public bool IsActive { get; set; }
}
