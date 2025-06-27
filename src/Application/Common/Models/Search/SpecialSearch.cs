namespace Application.Common.Models.Search;

/// <summary>
/// Search parameters for specials
/// </summary>
public class SpecialSearch
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public long? VenueId { get; set; }
    public string? StartDate { get; set; } // Format: "yyyy-MM-dd"
    public string? EndDate { get; set; }   // Format: "yyyy-MM-dd"
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusInMeters { get; set; } = 5000; // Default 5km
    public bool ActiveOnly { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "venue"; // "venue", "distance", "category", "date"
    public string? SortOrder { get; set; } = "asc"; // "asc", "desc"
}
