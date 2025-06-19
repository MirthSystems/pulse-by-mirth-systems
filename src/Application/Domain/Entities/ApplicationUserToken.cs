using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationUserToken : IdentityUserToken<long>
{
    public ApplicationUser User { get; set; } = new ApplicationUser();
}
