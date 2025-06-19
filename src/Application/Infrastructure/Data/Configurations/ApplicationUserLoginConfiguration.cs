using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        #region Entity Configuration
        builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });

        builder.ToTable("application_user_logins");

        builder.Property(l => l.LoginProvider)
            .HasMaxLength(128);
        
        builder.Property(l => l.ProviderKey)
            .HasMaxLength(128);

        builder.Property(ul => ul.LoginProvider)
            .HasColumnName("login_provider");
        
        builder.Property(ul => ul.ProviderKey)
            .HasColumnName("provider_key");
        
        builder.Property(ul => ul.ProviderDisplayName)
            .HasColumnName("provider_display_name");
        
        builder.Property(ul => ul.UserId)
            .HasColumnName("user_id");
        #endregion
    }
}
