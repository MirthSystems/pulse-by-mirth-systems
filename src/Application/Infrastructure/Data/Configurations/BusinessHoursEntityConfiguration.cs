using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Application.Domain.Entities;
using Application.Domain.Enums;

namespace Application.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for BusinessHours entity
/// </summary>
public class BusinessHoursEntityConfiguration : IEntityTypeConfiguration<BusinessHoursEntity>
{
    public void Configure(EntityTypeBuilder<BusinessHoursEntity> builder)
    {
        builder.HasIndex(bh => new { bh.VenueId, bh.DayOfWeekId })
               .IsUnique();

        builder.HasKey(bh => bh.Id);

        builder.HasOne(bh => bh.Venue)
               .WithMany(v => v.BusinessHours)
               .HasForeignKey(bh => bh.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bh => bh.DayOfWeek)
               .WithMany(dow => dow.BusinessHours)
               .HasForeignKey(bh => bh.DayOfWeekId)
               .OnDelete(DeleteBehavior.Restrict);

        #region Data Seed
        builder.HasData(
            #region Bullfrog Brewery Hours (Venue ID 1)
            new BusinessHoursEntity
            {
                Id = 1,
                VenueId = 1,
                DayOfWeekId = Days.Sunday, // Sunday
                OpenTime = new LocalTime(10, 0),
                CloseTime = new LocalTime(15, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 2,
                VenueId = 1,
                DayOfWeekId = Days.Monday, // Monday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 3,
                VenueId = 1,
                DayOfWeekId = Days.Tuesday, // Tuesday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 4,
                VenueId = 1,
                DayOfWeekId = Days.Wednesday, // Wednesday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 5,
                VenueId = 1,
                DayOfWeekId = Days.Thursday, // Thursday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 6,
                VenueId = 1,
                DayOfWeekId = Days.Friday, // Friday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 7,
                VenueId = 1,
                DayOfWeekId = Days.Saturday, // Saturday
                OpenTime = new LocalTime(11, 30),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            },
            #endregion

            #region The Brickyard Restaurant & Ale House Hours (Venue ID 2)
            new BusinessHoursEntity
            {
                Id = 8,
                VenueId = 2,
                DayOfWeekId = Days.Sunday, // Sunday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(23, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 9,
                VenueId = 2,
                DayOfWeekId = Days.Monday, // Monday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 10,
                VenueId = 2,
                DayOfWeekId = Days.Tuesday, // Tuesday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 11,
                VenueId = 2,
                DayOfWeekId = Days.Wednesday, // Wednesday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 12,
                VenueId = 2,
                DayOfWeekId = Days.Thursday, // Thursday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(0, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 13,
                VenueId = 2,
                DayOfWeekId = Days.Friday, // Friday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(2, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 14,
                VenueId = 2,
                DayOfWeekId = Days.Saturday, // Saturday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(2, 0),
                IsClosed = false
            },
            #endregion

            #region The Crooked Goose Hours (Venue ID 3)
            new BusinessHoursEntity
            {
                Id = 15,
                VenueId = 3,
                DayOfWeekId = Days.Sunday, // Sunday
                OpenTime = new LocalTime(10, 0),
                CloseTime = new LocalTime(14, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 16,
                VenueId = 3,
                DayOfWeekId = Days.Monday, // Monday - Closed
                OpenTime = null,
                CloseTime = null,
                IsClosed = true
            }, 
            new BusinessHoursEntity
            {
                Id = 17,
                VenueId = 3,
                DayOfWeekId = Days.Tuesday, // Tuesday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(21, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 18,
                VenueId = 3,
                DayOfWeekId = Days.Wednesday, // Wednesday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(21, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 19,
                VenueId = 3,
                DayOfWeekId = Days.Thursday, // Thursday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(21, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 20,
                VenueId = 3,
                DayOfWeekId = Days.Friday, // Friday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }, 
            new BusinessHoursEntity
            {
                Id = 21,
                VenueId = 3,
                DayOfWeekId = Days.Saturday, // Saturday
                OpenTime = new LocalTime(11, 0),
                CloseTime = new LocalTime(22, 0),
                IsClosed = false
            }
            #endregion
        );
        #endregion
    }
}
