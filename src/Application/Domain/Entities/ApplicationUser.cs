using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public List<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();
    public List<ApplicationUserLogin> Logins { get; set; } = new List<ApplicationUserLogin>();
    public List<ApplicationUserToken> Tokens { get; set; } = new List<ApplicationUserToken>();
    public List<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
