using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        #region Entity Configuration
        builder.HasKey(r => new { r.UserId, r.RoleId });

        builder.ToTable("application_user_roles");

        builder.Property(ur => ur.UserId)
            .HasColumnName("user_id");
        
        builder.Property(ur => ur.RoleId)
            .HasColumnName("role_id");
        #endregion
    }
}
