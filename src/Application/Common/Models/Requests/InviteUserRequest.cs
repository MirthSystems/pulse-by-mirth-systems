namespace Application.Common.Models.Requests;

/// <summary>
/// Request model for inviting a user
/// </summary>
public class InviteUserRequest
{
    public string Email { get; set; } = null!;
    public string Permission { get; set; } = null!;
    public string? Notes { get; set; }
    public int ExpirationDays { get; set; } = 7;
}
