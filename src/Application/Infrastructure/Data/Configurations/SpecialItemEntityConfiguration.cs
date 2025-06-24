using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

public class SpecialItemEntityConfiguration : IEntityTypeConfiguration<SpecialItemEntity>
{
    public void Configure(EntityTypeBuilder<SpecialItemEntity> builder)
    {
        builder.HasKey(si => si.Id);

        builder.HasOne(si => si.SpecialsMenu)
               .WithMany(sm => sm.SpecialItems)
               .HasForeignKey(si => si.SpecialsMenuId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Category)
               .WithMany(sic => sic.SpecialItems)
               .HasForeignKey(si => si.SpecialItemCategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        #region Data Seed
        builder.HasData(
            // Items for Live Music Weekend (Menu ID 1)            
            new SpecialItemEntity
            {
                Id = 1,
                SpecialsMenuId = 1,
                SpecialItemCategoryId = 3, // Entertainment
                Name = "Live Music Friday/Saturday",
                Description = "Live music showcasing the best in local, regional, and national talent. Various genres from rock to jazz.",
                IsActive = true
            },
            
            // Items for Happy Hour (Menu ID 2)
            new SpecialItemEntity
            {
                Id = 2,
                SpecialsMenuId = 2,
                SpecialItemCategoryId = 2, // Drink
                Name = "$1 Off Draft Beers",
                Description = "Enjoy $1 off all draft beers during happy hour.",
                IsActive = true
            },
            new SpecialItemEntity
            {
                Id = 3,
                SpecialsMenuId = 2,
                SpecialItemCategoryId = 2, // Drink
                Name = "$5 House Wines",
                Description = "All house wines for just $5 during happy hour.",
                IsActive = true
            },
            
            // Items for Weekly Food Special (Menu ID 3)
            new SpecialItemEntity
            {
                Id = 4,
                SpecialsMenuId = 3,
                SpecialItemCategoryId = 1, // Food
                Name = "Sweet & Spicy Chicken Sandwich",
                Description = "Sweet and spicy chicken sandwich with sweet n spicy sauce, lettuce, and pickles.",
                IsActive = true
            },
            
            // Items for Wednesday Night Quizzo (Menu ID 4)
            new SpecialItemEntity
            {
                Id = 5,
                SpecialsMenuId = 4,
                SpecialItemCategoryId = 3, // Entertainment
                Name = "Pub Trivia",
                Description = "Pub Trivia with first and second place prizes. Sponsored by Bacardi Oakheart.",
                IsActive = true
            },
            
            // Items for Mug Club Tuesday (Menu ID 5)
            new SpecialItemEntity
            {
                Id = 6,
                SpecialsMenuId = 5,
                SpecialItemCategoryId = 2, // Drink
                Name = "Free First Beer for Mug Club Members",
                Description = "Our valued Mug club members enjoy their First beer, of their choice, on US!!",
                IsActive = true
            },
            
            // Items for Sunday Brunch (Menu ID 6)
            new SpecialItemEntity
            {
                Id = 7,
                SpecialsMenuId = 6,
                SpecialItemCategoryId = 1, // Food
                Name = "Sunday Brunch Menu",
                Description = "Special brunch menu served from 10am to 2pm every Sunday.",
                IsActive = true
            },
            
            // Items for Cocktail Hour (Menu ID 7)
            new SpecialItemEntity
            {
                Id = 8,
                SpecialsMenuId = 7,
                SpecialItemCategoryId = 2, // Drink
                Name = "Discounted Craft Cocktails",
                Description = "Enjoy our specially crafted cocktails at a reduced price.",
                IsActive = true
            }
        );
        #endregion
    }
}
