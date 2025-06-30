using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.Auth;

/// <summary>
/// Request model for updating user permissions
/// </summary>
public class UpdatePermissionRequest
{
    [Required]
    [MaxLength(100)]
    public string Permission { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
