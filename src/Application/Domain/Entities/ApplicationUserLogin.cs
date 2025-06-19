using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationUserLogin : IdentityUserLogin<long>
{
    public ApplicationUser User { get; set; } = new ApplicationUser();
}
