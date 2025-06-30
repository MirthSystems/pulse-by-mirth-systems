using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Location;

/// <summary>
/// Request to validate a complete address from components
/// </summary>
public class ValidateAddressRequest
{
    /// <summary>
    /// Street address (e.g., "123 Main St")
    /// </summary>
    [MaxLength(200)]
    public string? StreetAddress { get; set; }
    
    /// <summary>
    /// Secondary address (e.g., "Apt 4B", "Suite 200")
    /// </summary>
    [MaxLength(100)]
    public string? SecondaryAddress { get; set; }
    
    /// <summary>
    /// City/locality (e.g., "New York")
    /// </summary>
    [MaxLength(100)]
    public string? Locality { get; set; }
    
    /// <summary>
    /// State/region (e.g., "NY", "California")
    /// </summary>
    [MaxLength(100)]
    public string? Region { get; set; }
    
    /// <summary>
    /// Postal/zip code (e.g., "10001", "17701")
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country (e.g., "United States", "USA")
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }
}
