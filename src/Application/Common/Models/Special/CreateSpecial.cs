using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Special;

/// <summary>
/// Model for creating a new special
/// </summary>
public class CreateSpecial : SpecialBase
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "VenueId must be a positive integer")]
    public long VenueId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "SpecialCategoryId must be a positive integer")]
    public int SpecialCategoryId { get; set; }
}
