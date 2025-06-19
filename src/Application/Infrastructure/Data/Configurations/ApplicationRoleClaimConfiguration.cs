using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        #region Entity Configuration
        builder.HasKey(rc => rc.Id);

        builder.ToTable("application_role_claims");

        builder.Property(rc => rc.Id)
            .HasColumnName("id");
        
        builder.Property(rc => rc.RoleId)
            .HasColumnName("role_id");
        
        builder.Property(rc => rc.ClaimType)
            .HasColumnName("claim_type");
        
        builder.Property(rc => rc.ClaimValue)
            .HasColumnName("claim_value");
        #endregion
    }
}
