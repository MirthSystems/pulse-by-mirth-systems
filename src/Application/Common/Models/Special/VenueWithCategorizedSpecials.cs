namespace Application.Common.Models.Special;

/// <summary>
/// Categorized specials by type (food, drink, entertainment)
/// </summary>
public class CategorizedSpecials
{
    public IEnumerable<SpecialSummary> Food { get; set; } = new List<SpecialSummary>();
    public IEnumerable<SpecialSummary> Drink { get; set; } = new List<SpecialSummary>();
    public IEnumerable<SpecialSummary> Entertainment { get; set; } = new List<SpecialSummary>();
}

/// <summary>
/// Venue with categorized active specials for enhanced search results
/// </summary>
public class VenueWithCategorizedSpecials
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public string? SecondaryAddress { get; set; }
    public string Locality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryIcon { get; set; }
    public double? DistanceInMeters { get; set; }
    public CategorizedSpecials Specials { get; set; } = new();
    public int TotalSpecialCount { get; set; }
}
