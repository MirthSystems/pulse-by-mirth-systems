using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationUserClaim : IdentityUserClaim<long>
{
    public ApplicationUser User { get; set; } = new ApplicationUser();
}
