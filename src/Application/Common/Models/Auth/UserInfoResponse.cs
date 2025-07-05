namespace Application.Common.Models.Auth;

/// <summary>
/// Response model for user information
/// </summary>
public class UserInfoResponse
{
    public long Id { get; set; }
    public required string Sub { get; set; }
    public required string Email { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
