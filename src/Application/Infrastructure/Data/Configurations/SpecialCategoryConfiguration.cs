using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Data.Configurations;

public class SpecialCategoryConfiguration : IEntityTypeConfiguration<SpecialCategory>
{
    public void Configure(EntityTypeBuilder<SpecialCategory> builder)
    {
        builder.HasIndex(sc => sc.Name)
               .IsUnique();

        builder.HasKey(sc => sc.Id);

        #region Data Seed
        builder.HasData(
            new SpecialCategory
            {
                Id = 1,
                Name = "Food",
                Description = "Food specials, appetizers, and meal deals",
                Icon = "🍔",
                SortOrder = 1,
            },
            new SpecialCategory
            {
                Id = 2,
                Name = "Drink",
                Description = "Drink specials, happy hours, and beverage promotions",
                Icon = "🍺",
                SortOrder = 2,
            },
            new SpecialCategory
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
