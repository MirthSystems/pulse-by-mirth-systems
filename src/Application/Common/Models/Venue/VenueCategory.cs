namespace Application.Common.Models.Venue;

/// <summary>
/// Venue category model
/// </summary>
public class VenueCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}
