using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Auth;

/// <summary>
/// Request model for updating user information from frontend token
/// </summary>
public class UserInfo
{
    [Required]
    [MaxLength(255)]
    public required string Sub { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Nickname { get; set; }

    [MaxLength(500)]
    public string? Picture { get; set; }

    public bool EmailVerified { get; set; }
}
