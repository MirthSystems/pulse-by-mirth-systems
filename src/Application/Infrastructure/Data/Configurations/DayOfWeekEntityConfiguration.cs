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
        builder.HasIndex(d => d.Enum)
               .IsUnique();

        builder.HasIndex(d => d.IsoNumber)
               .IsUnique();

        builder.HasKey(d => d.Id);

        #region Data Seed
        builder.HasData(
            new DayOfWeekEntity
            {
                Id = 1,
                Enum = DayOfWeekEnum.Sunday,
                Name = "Sunday",
                ShortName = "SUN",
                IsoNumber = 7,
                IsWeekday = false,
                SortOrder = 1,            
            },
            new DayOfWeekEntity
            {
                Id = 2,
                Enum = DayOfWeekEnum.Monday,
                Name = "Monday",
                ShortName = "MON",
                IsoNumber = 1,
                IsWeekday = true,
                SortOrder = 2,
            },
            new DayOfWeekEntity
            {
                Id = 3,
                Enum = DayOfWeekEnum.Tuesday,
                Name = "Tuesday",
                ShortName = "TUE",
                IsoNumber = 2,
                IsWeekday = true,
                SortOrder = 3,              
            },
            new DayOfWeekEntity
            {
                Id = 4,
                Enum = DayOfWeekEnum.Wednesday,
                Name = "Wednesday",
                ShortName = "WED",
                IsoNumber = 3,
                IsWeekday = true,
                SortOrder = 4,
            },
            new DayOfWeekEntity
            {
                Id = 5,
                Enum = DayOfWeekEnum.Thursday,
                Name = "Thursday",
                ShortName = "THU",
                IsoNumber = 4,
                IsWeekday = true,
                SortOrder = 5,
            },
            new DayOfWeekEntity
            {
                Id = 6,
                Enum = DayOfWeekEnum.Friday,
                Name = "Friday",
                ShortName = "FRI",
                IsoNumber = 5,
                IsWeekday = true,
                SortOrder = 6,
            },
            new DayOfWeekEntity
            {
                Id = 7,
                Enum = DayOfWeekEnum.Saturday,
                Name = "Saturday",
                ShortName = "SAT",
                IsoNumber = 6,
                IsWeekday = false,
                SortOrder = 7,
            }
        );
        #endregion
    }
}
