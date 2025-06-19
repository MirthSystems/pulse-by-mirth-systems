using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationUserRole : IdentityUserRole<long>
{
    public ApplicationUser User { get; set; } = new ApplicationUser();
    public ApplicationRole Role { get; set; } = new ApplicationRole();
}
