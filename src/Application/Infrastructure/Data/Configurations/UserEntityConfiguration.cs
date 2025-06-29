using Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for User entity
/// </summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes for performance
        builder.HasIndex(u => u.Sub)
               .IsUnique()
               .HasDatabaseName("IX_users_sub");

        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("IX_users_email");

        builder.HasIndex(u => u.IsActive)
               .HasDatabaseName("IX_users_is_active");

        // Navigation properties
        builder.HasMany(u => u.VenuePermissions)
               .WithOne(uvp => uvp.User)
               .HasForeignKey(uvp => uvp.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SentInvitations)
               .WithOne(vi => vi.InvitedByUser)
               .HasForeignKey(vi => vi.InvitedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedInvitations)
               .WithOne(vi => vi.AcceptedByUser)
               .HasForeignKey(vi => vi.AcceptedByUserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
