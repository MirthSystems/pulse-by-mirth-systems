using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        #region Entity Configuration
        builder.HasKey(r => r.Id);

        builder.ToTable("application_roles");

        builder.HasIndex(r => r.NormalizedName)
            .HasDatabaseName("role_name_index")
            .IsUnique();

        builder.Property(r => r.ConcurrencyStamp)
            .IsConcurrencyToken();

        builder.Property(r => r.Name)
            .HasMaxLength(256);
        
        builder.Property(r => r.NormalizedName)
            .HasMaxLength(256);

        builder.Property(r => r.Id)
            .HasColumnName("id");
        
        builder.Property(r => r.Name)
            .HasColumnName("name");
        
        builder.Property(r => r.NormalizedName)
            .HasColumnName("normalized_name");
        
        builder.Property(r => r.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.HasMany(e => e.RoleClaims)
            .WithOne(e => e.Role)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();
        #endregion
    }
}
