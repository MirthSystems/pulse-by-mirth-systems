using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;
using Application.Domain.Enums;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for DayOfWeek entity
/// </summary>
public class DayOfWeekEntityConfiguration : IEntityTypeConfiguration<DayOfWeekEntity>
{
    public void Configure(EntityTypeBuilder<DayOfWeekEntity> builder)
    {
        builder.HasIndex(d => d.IsoNumber)
               .IsUnique();

        builder.HasKey(d => d.Id);

        #region Data Seed
        builder.HasData(
            new DayOfWeekEntity
            {
                Id = Days.Sunday,
                Name = "Sunday",
                ShortName = "SUN",
                IsoNumber = 7,
                IsWeekday = false,
                SortOrder = 0,            
            },
            new DayOfWeekEntity
            {
                Id = Days.Monday,
                Name = "Monday",
                ShortName = "MON",
                IsoNumber = 1,
                IsWeekday = true,
                SortOrder = 1,
            },
            new DayOfWeekEntity
            {
                Id = Days.Tuesday,
                Name = "Tuesday",
                ShortName = "TUE",
                IsoNumber = 2,
                IsWeekday = true,
                SortOrder = 2,              
            },
            new DayOfWeekEntity
            {
                Id = Days.Wednesday,
                Name = "Wednesday",
                ShortName = "WED",
                IsoNumber = 3,
                IsWeekday = true,
                SortOrder = 3,
            },
            new DayOfWeekEntity
            {
                Id = Days.Thursday,
                Name = "Thursday",
                ShortName = "THU",
                IsoNumber = 4,
                IsWeekday = true,
                SortOrder = 4,
            },
            new DayOfWeekEntity
            {
                Id = Days.Friday,
                Name = "Friday",
                ShortName = "FRI",
                IsoNumber = 5,
                IsWeekday = true,
                SortOrder = 5,
            },
            new DayOfWeekEntity
            {
                Id = Days.Saturday,
                Name = "Saturday",
                ShortName = "SAT",
                IsoNumber = 6,
                IsWeekday = false,
                SortOrder = 6,
            }
        );
        #endregion
    }
}
