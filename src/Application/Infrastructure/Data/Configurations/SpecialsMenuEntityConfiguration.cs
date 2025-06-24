using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NodaTime;

namespace Application.Infrastructure.Data.Configurations;

public class SpecialsMenuEntityConfiguration : IEntityTypeConfiguration<SpecialsMenuEntity>
{
    public void Configure(EntityTypeBuilder<SpecialsMenuEntity> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.HasOne(sm => sm.Venue)
               .WithMany(v => v.SpecialsMenus)
               .HasForeignKey(sm => sm.VenueId)
               .OnDelete(DeleteBehavior.Cascade);

        #region Data Seed
        builder.HasData(
            new SpecialsMenuEntity
            {
                Id = 1,
                VenueId = 1, // Bullfrog Brewery
                Title = "Live Music Weekend",
                Description = "Live music showcasing the best in local, regional, and national talent. Various genres from rock to jazz.",
                StartDate = new LocalDate(2025, 5, 3), // First Saturday in May 2025
                StartTime = new LocalTime(21, 0), // 9:00 PM
                EndTime = new LocalTime(23, 0), // 11:00 PM
                IsRecurring = true,
                CronSchedule = "0 21 * * 5,6", // Every Friday and Saturday at 9 PM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 2,
                VenueId = 1, // Bullfrog Brewery
                Title = "Happy Hour",
                Description = "Enjoy $1 off all draft beers and $5 house wines.",
                StartDate = new LocalDate(2025, 5, 1), // May 1st, 2025
                StartTime = new LocalTime(16, 0), // 4:00 PM
                EndTime = new LocalTime(18, 0), // 6:00 PM
                IsRecurring = true,
                CronSchedule = "0 16 * * 1-5", // Weekdays at 4 PM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 3,
                VenueId = 2, // The Brickyard
                Title = "Weekly Food Special",
                Description = "Sweet and spicy chicken sandwich with sweet n spicy sauce, lettuce, and pickles.",
                StartDate = new LocalDate(2025, 5, 20), // Two days before current date
                StartTime = new LocalTime(11, 0), // 11:00 AM
                EndTime = new LocalTime(22, 0), // 10:00 PM
                EndDate = new LocalDate(2025, 5, 27), // One week after start
                IsRecurring = false,
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 4,
                VenueId = 2, // The Brickyard
                Title = "Wednesday Night Quizzo",
                Description = "Pub Trivia with first and second place prizes. Sponsored by Bacardi Oakheart.",
                StartDate = new LocalDate(2025, 5, 22), // Current date (Wednesday)
                StartTime = new LocalTime(21, 0), // 9:00 PM
                EndTime = new LocalTime(23, 0), // 11:00 PM
                IsRecurring = true,
                CronSchedule = "0 21 * * 3", // Every Wednesday at 9 PM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 5,
                VenueId = 2, // The Brickyard
                Title = "Mug Club Tuesday",
                Description = "Every Tuesday is Mug Club Night. Our valued Mug club members enjoy their First beer, of their choice, on US!!",
                StartDate = new LocalDate(2025, 5, 21), // Tuesday before current date
                StartTime = new LocalTime(11, 0), // 11:00 AM
                EndTime = new LocalTime(23, 0), // 11:00 PM
                IsRecurring = true,
                CronSchedule = "0 11 * * 2", // Every Tuesday at 11 AM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 6,
                VenueId = 3, // The Crooked Goose
                Title = "Sunday Brunch",
                Description = "Special brunch menu served from 10am to 2pm every Sunday.",
                StartDate = new LocalDate(2025, 5, 18), // Previous Sunday
                StartTime = new LocalTime(10, 0), // 10:00 AM
                EndTime = new LocalTime(14, 0), // 2:00 PM
                IsRecurring = true,
                CronSchedule = "0 10 * * 0", // Every Sunday at 10 AM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            },
            new SpecialsMenuEntity
            {
                Id = 7,
                VenueId = 3, // The Crooked Goose
                Title = "Cocktail Hour",
                Description = "Enjoy our specially crafted cocktails at a reduced price.",
                StartDate = new LocalDate(2025, 5, 21), // Tuesday before current date                    
                StartTime = new LocalTime(16, 0), // 4:00 PM
                EndTime = new LocalTime(18, 0), // 6:00 PM
                IsRecurring = true,
                CronSchedule = "0 16 * * 2-6", // Tuesday-Saturday at 4 PM
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            }
        );
        #endregion
    }
}
