using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        #region Entity Configuration
        builder.HasKey(uc => uc.Id);

        builder.ToTable("application_user_claims");

        builder.Property(uc => uc.Id)
            .HasColumnName("id");
        
        builder.Property(uc => uc.UserId)
            .HasColumnName("user_id");
        
        builder.Property(uc => uc.ClaimType)
            .HasColumnName("claim_type");
        
        builder.Property(uc => uc.ClaimValue)
            .HasColumnName("claim_value");
        #endregion
    }
}
