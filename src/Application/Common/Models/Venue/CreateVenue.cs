using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Venue;

/// <summary>
/// Model for creating a new venue
/// </summary>
public class CreateVenue : VenueBase
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer")]
    public int CategoryId { get; set; }
}
