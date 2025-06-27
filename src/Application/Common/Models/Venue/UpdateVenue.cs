using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Venue;

/// <summary>
/// Model for updating an existing venue
/// </summary>
public class UpdateVenue : VenueBase
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer")]
    public int CategoryId { get; set; }
    
    public List<BusinessHoursRequest>? BusinessHours { get; set; }
}
