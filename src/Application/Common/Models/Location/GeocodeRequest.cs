using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Location;

/// <summary>
/// Address geocoding request
/// </summary>
public class GeocodeRequest
{
    /// <summary>
    /// The address to geocode
    /// </summary>
    [Required]
    [MinLength(3, ErrorMessage = "Address must be at least 3 characters long")]
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional city to improve geocoding accuracy
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }
    
    /// <summary>
    /// Optional region/state to improve geocoding accuracy
    /// </summary>
    [MaxLength(100)]
    public string? Region { get; set; }
    
    /// <summary>
    /// Optional postal code to improve geocoding accuracy
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Optional country to improve geocoding accuracy
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }
}
