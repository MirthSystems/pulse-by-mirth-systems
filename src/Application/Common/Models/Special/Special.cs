namespace Application.Common.Models.Special;

/// <summary>
/// Model for special responses
/// </summary>
public class Special : SpecialBase
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public int SpecialCategoryId { get; set; }
    public VenueSpecial? Venue { get; set; }
    public SpecialCategory? Category { get; set; }
}
