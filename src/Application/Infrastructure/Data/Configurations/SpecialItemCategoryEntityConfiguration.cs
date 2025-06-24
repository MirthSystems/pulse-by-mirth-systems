using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

public class SpecialItemCategoryEntityConfiguration : IEntityTypeConfiguration<SpecialItemCategoryEntity>
{
    public void Configure(EntityTypeBuilder<SpecialItemCategoryEntity> builder)
    {
        builder.HasIndex(sc => sc.Name)
               .IsUnique();

        builder.HasKey(sc => sc.Id);

        #region Data Seed
        builder.HasData(
            new SpecialItemCategoryEntity
            {
                Id = 1,
                Name = "Food",
                Description = "Food specials, appetizers, and meal deals",
                Icon = "🍔",
                SortOrder = 1,
            },
            new SpecialItemCategoryEntity
            {
                Id = 2,
                Name = "Drink",
                Description = "Drink specials, happy hours, and beverage promotions",
                Icon = "🍺",
                SortOrder = 2,
            },
            new SpecialItemCategoryEntity
            {
                Id = 3,
                Name = "Entertainment",
                Description = "Live music, DJs, trivia, karaoke, and other events",
                Icon = "🎵",
                SortOrder = 3,
            }
        );
        #endregion
    }
}
