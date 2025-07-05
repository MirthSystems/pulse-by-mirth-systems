using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Auth;

public class CreateInvitationRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public long VenueId { get; set; }
    
    [Required]
    public required string Permission { get; set; }
    
    public string? Notes { get; set; }
    
    [Required]
    [EmailAddress]
    public required string SenderEmail { get; set; }
}
