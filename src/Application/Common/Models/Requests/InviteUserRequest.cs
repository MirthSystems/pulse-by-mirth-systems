using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Requests;

/// <summary>
/// Request model for inviting a user
/// </summary>
public class InviteUserRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required string Permission { get; set; }
    
    public string? Notes { get; set; }
    public int ExpirationDays { get; set; } = 7;
}
