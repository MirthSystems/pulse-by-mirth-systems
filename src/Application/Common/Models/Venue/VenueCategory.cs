namespace Application.Common.Models.Venue;

/// <summary>
/// Venue category model
/// </summary>
public class VenueCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}
