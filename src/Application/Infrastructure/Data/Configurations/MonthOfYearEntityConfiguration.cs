using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Application.Domain.Entities;
using Application.Domain.Enums;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for MonthOfYear entity
/// </summary>
public class MonthOfYearEntityConfiguration : IEntityTypeConfiguration<MonthOfYearEntity>
{
    public void Configure(EntityTypeBuilder<MonthOfYearEntity> builder)
    {
        builder.HasIndex(m => m.IsoNumber)
               .IsUnique();

        builder.HasKey(m => m.Id);

        #region Data Seed
        builder.HasData(
            new MonthOfYearEntity
            {
                Id = Months.January,
                Name = "January",
                ShortName = "JAN",
                IsoNumber = 01,
                SortOrder = 0,
            },
            new MonthOfYearEntity
            {
                Id = Months.February,
                Name = "February",
                ShortName = "FEB",
                IsoNumber = 02,
                SortOrder = 1,
            },
            new MonthOfYearEntity
            {
                Id = Months.March,
                Name = "March",
                ShortName = "MAR",
                IsoNumber = 03,
                SortOrder = 2,
            },
            new MonthOfYearEntity
            {
                Id = Months.April,
                Name = "April",
                ShortName = "APR",
                IsoNumber = 04,
                SortOrder = 3,
            },
            new MonthOfYearEntity
            {
                Id = Months.May,
                Name = "May",
                ShortName = "MAY",
                IsoNumber = 05,
                SortOrder = 4,
            },
            new MonthOfYearEntity
            {
                Id = Months.June,
                Name = "June",
                ShortName = "JUN",
                IsoNumber = 06,
                SortOrder = 5,
            },
            new MonthOfYearEntity
            {
                Id = Months.July,
                Name = "July",
                ShortName = "JUL",
                IsoNumber = 07,
                SortOrder = 6,
            },
            new MonthOfYearEntity
            {
                Id = Months.August,
                Name = "August",
                ShortName = "AUG",
                IsoNumber = 08,
                SortOrder = 7,
            },
            new MonthOfYearEntity
            {
                Id = Months.September,
                Name = "September",
                ShortName = "SEP",
                IsoNumber = 09,
                SortOrder = 8,
            },
            new MonthOfYearEntity
            {
                Id = Months.October,
                Name = "October",
                ShortName = "OCT",
                IsoNumber = 10,
                SortOrder = 9,
            },
            new MonthOfYearEntity
            {
                Id = Months.November,
                Name = "November",
                ShortName = "NOV",
                IsoNumber = 11,
                SortOrder = 10,
            },
            new MonthOfYearEntity
            {
                Id = Months.December,
                Name = "December",
                ShortName = "DEC",
                IsoNumber = 12,
                SortOrder = 11,
            }
        );
        #endregion
    }
}
