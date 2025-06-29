using Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for VenueInvitation entity
/// </summary>
public class VenueInvitationEntityConfiguration : IEntityTypeConfiguration<VenueInvitationEntity>
{
    public void Configure(EntityTypeBuilder<VenueInvitationEntity> builder)
    {
        // Primary key
        builder.HasKey(vi => vi.Id);

        // Indexes for performance and uniqueness
        builder.HasIndex(vi => new { vi.Email, vi.VenueId, vi.IsActive })
               .HasDatabaseName("IX_venue_invitations_email_venue_active");

        builder.HasIndex(vi => vi.VenueId)
               .HasDatabaseName("IX_venue_invitations_venue_id");

        builder.HasIndex(vi => vi.InvitedByUserId)
               .HasDatabaseName("IX_venue_invitations_invited_by");

        builder.HasIndex(vi => vi.ExpiresAt)
               .HasDatabaseName("IX_venue_invitations_expires_at");

        builder.HasIndex(vi => new { vi.IsActive, vi.AcceptedAt })
               .HasDatabaseName("IX_venue_invitations_active_accepted");

        // Foreign key relationships
        builder.HasOne(vi => vi.Venue)
               .WithMany()
               .HasForeignKey(vi => vi.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vi => vi.InvitedByUser)
               .WithMany(u => u.SentInvitations)
               .HasForeignKey(vi => vi.InvitedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vi => vi.AcceptedByUser)
               .WithMany(u => u.ReceivedInvitations)
               .HasForeignKey(vi => vi.AcceptedByUserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
