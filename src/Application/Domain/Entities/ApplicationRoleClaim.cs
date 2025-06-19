using Microsoft.AspNetCore.Identity;

namespace Application.Domain.Entities;

public class ApplicationRoleClaim : IdentityRoleClaim<long>
{
    public ApplicationRole Role { get; set; } = new ApplicationRole();
}
