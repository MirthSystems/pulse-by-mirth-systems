namespace Application.Common.Models.Special;

/// <summary>
/// Enhanced special search request parameters
/// </summary>
public class EnhancedSpecialSearch
{
    public string? SearchTerm { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusInMeters { get; set; } = 5000; // Default 5km
    public string? Date { get; set; } // Format: "yyyy-MM-dd", defaults to today
    public string? Time { get; set; } // Format: "HH:mm", defaults to current time
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "distance"; // "distance", "name", "special_count"
    public string? SortOrder { get; set; } = "asc"; // "asc", "desc"
}
