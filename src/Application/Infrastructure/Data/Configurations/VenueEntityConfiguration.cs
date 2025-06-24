using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using NodaTime;
using Application.Domain.Entities;

namespace Application.Infrastructure.Data.Configurations;

public class VenueEntityConfiguration : IEntityTypeConfiguration<VenueEntity>
{
    public void Configure(EntityTypeBuilder<VenueEntity> builder)
    {
        builder.HasIndex(v => v.Name);
        builder.HasIndex(v => v.Location)
               .HasMethod("GIST");

        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.Category)
               .WithMany(vc => vc.Venues)
               .HasForeignKey(v => v.CategoryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();        
        builder.HasMany(v => v.BusinessHours)
               .WithOne(bh => bh.Venue)
               .HasForeignKey(bh => bh.VenueId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(v => v.Specials)
               .WithOne(s => s.Venue)
               .HasForeignKey(s => s.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        #region Data Seed
        builder.HasData(
            // Bullfrog Brewery
            new VenueEntity
            {
                Id = 1,
                CategoryId = 7, // Brewery
                Name = "Bullfrog Brewery",
                Description = "Local craft brewery featuring house-made beers, pub fare, and live entertainment in a cozy atmosphere.",
                PhoneNumber = "(570) 326-4700",
                Website = "https://bullfrogbrewery.com",
                Email = "info@bullfrogbrewery.com",
                StreetAddress = "229 W 4th St",
                Locality = "Williamsport",
                Region = "PA",
                PostalCode = "17701",
                Country = "United States",
                Location = new Point(-77.0057192, 41.240432)
                {
                    SRID = 4326
                },
                IsActive = true
            },
            // The Brickyard Restaurant & Ale House
            new VenueEntity
            {
                Id = 2,
                CategoryId = 1, // Restaurant
                Name = "The Brickyard Restaurant & Ale House",
                Description = "Family-friendly restaurant and ale house serving American cuisine with a great selection of craft beers and cocktails.",
                PhoneNumber = "(570) 322-3876",
                Website = "https://thebrickyard.net",
                Email = "info@thebrickyard.net",
                StreetAddress = "343 Pine St",
                Locality = "Williamsport",
                Region = "PA",
                PostalCode = "17701",
                Country = "United States",
                Location = new Point(-77.0037646, 41.2409825)
                {
                    SRID = 4326
                },
                IsActive = true
            },
            // The Crooked Goose
            new VenueEntity
            {
                Id = 3,
                CategoryId = 2, // Bar
                Name = "The Crooked Goose",
                Description = "Upscale gastropub featuring craft cocktails, local beers, and elevated bar food in a sophisticated atmosphere.",
                PhoneNumber = "(570) 360-7435",
                Website = "https://thecrookedgoose.com",
                Email = "info@thecrookedgoose.com",
                StreetAddress = "215 W 4th St",
                Locality = "Williamsport",
                Region = "PA",
                PostalCode = "17701",
                Country = "United States",
                Location = new Point(-77.0047521, 41.2407201)
                {
                    SRID = 4326
                },
                IsActive = true
            }
        );
        #endregion
    }
}
