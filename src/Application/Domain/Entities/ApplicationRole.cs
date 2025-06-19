using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationRole : IdentityRole<long>
{
    public List<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

    public List<ApplicationRoleClaim> RoleClaims { get; set; } = new List<ApplicationRoleClaim>();
}
