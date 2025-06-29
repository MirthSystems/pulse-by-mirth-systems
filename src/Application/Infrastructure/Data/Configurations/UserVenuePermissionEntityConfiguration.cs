using Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserVenuePermission entity
/// </summary>
public class UserVenuePermissionEntityConfiguration : IEntityTypeConfiguration<UserVenuePermissionEntity>
{
    public void Configure(EntityTypeBuilder<UserVenuePermissionEntity> builder)
    {
        // Primary key
        builder.HasKey(uvp => uvp.Id);

        // Unique constraint - one permission record per user/venue combination
        builder.HasIndex(uvp => new { uvp.UserId, uvp.VenueId })
               .IsUnique()
               .HasDatabaseName("IX_user_venue_permissions_user_venue");

        // Additional indexes for performance
        builder.HasIndex(uvp => uvp.VenueId)
               .HasDatabaseName("IX_user_venue_permissions_venue_id");

        builder.HasIndex(uvp => uvp.IsActive)
               .HasDatabaseName("IX_user_venue_permissions_is_active");

        builder.HasIndex(uvp => new { uvp.VenueId, uvp.Name, uvp.IsActive })
               .HasDatabaseName("IX_user_venue_permissions_venue_role_active");

        // Foreign key relationships
        builder.HasOne(uvp => uvp.User)
               .WithMany(u => u.VenuePermissions)
               .HasForeignKey(uvp => uvp.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uvp => uvp.Venue)
               .WithMany()
               .HasForeignKey(uvp => uvp.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uvp => uvp.GrantedByUser)
               .WithMany()
               .HasForeignKey(uvp => uvp.GrantedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
