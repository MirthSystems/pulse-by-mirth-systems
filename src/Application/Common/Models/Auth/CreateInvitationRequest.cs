using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Auth;

public class CreateInvitationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public long VenueId { get; set; }
    
    [Required]
    public string Permission { get; set; } = string.Empty;
    
    public string? Notes { get; set; }
    
    [Required]
    [EmailAddress]
    public string SenderEmail { get; set; } = string.Empty;
}
