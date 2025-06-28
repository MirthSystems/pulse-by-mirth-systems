using Application.Domain.Entities;
using Application.Domain.Helpers;
using NodaTime;

namespace Application.UnitTests;

public class CronScheduleEvaluatorComprehensiveTests
{
    [Theory]
    [InlineData("0 10 * * 1", 2024, 1, 15, 10, 0, true)]  // Monday 10 AM special on Monday 10 AM - should be true
    [InlineData("0 10 * * 1", 2024, 1, 15, 15, 0, true)]  // Monday 10 AM special on Monday 3 PM - should be true (within range)
    [InlineData("0 10 * * 1", 2024, 1, 16, 10, 0, false)] // Monday 10 AM special on Tuesday 10 AM - should be false
    [InlineData("0 10 * * 1", 2024, 1, 15, 9, 0, false)]  // Monday 10 AM special on Monday 9 AM - should be false (before time)
    [InlineData("0 10 * * 1", 2024, 1, 15, 23, 0, false)] // Monday 10 AM special on Monday 11 PM - should be false (after time)
    public void IsSpecialCurrentlyActive_VariousScenarios_ShouldWorkCorrectly(
        string cronSchedule, int year, int month, int day, int hour, int minute, bool expectedResult)
    {
        // Arrange
        var special = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = cronSchedule,
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Act
        var testDateTime = new LocalDateTime(year, month, day, hour, minute);
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(special, testDateTime);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void IsSpecialCurrentlyActive_EveryDaySpecial_ShouldWorkCorrectly()
    {
        // Test a special that runs every day
        var everydaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 18 * * *", // 6 PM every day
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(18, 0), // 6:00 PM
            EndTime = new LocalTime(20, 0)    // 8:00 PM
        };

        // Should be active on any day within time range
        var mondayEvening = new LocalDateTime(2024, 1, 15, 19, 0); // Monday 7 PM
        var tuesdayEvening = new LocalDateTime(2024, 1, 16, 19, 0); // Tuesday 7 PM
        var saturdayEvening = new LocalDateTime(2024, 1, 20, 19, 0); // Saturday 7 PM
        var sundayEvening = new LocalDateTime(2024, 1, 21, 19, 0); // Sunday 7 PM

        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(everydaySpecial, mondayEvening));
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(everydaySpecial, tuesdayEvening));
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(everydaySpecial, saturdayEvening));
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(everydaySpecial, sundayEvening));
        
        // Should NOT be active outside time range
        var mondayMorning = new LocalDateTime(2024, 1, 15, 10, 0); // Monday 10 AM
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(everydaySpecial, mondayMorning));
    }

    [Fact]
    public void IsSpecialCurrentlyActive_WeekendSpecial_ShouldWorkCorrectly()
    {
        // Test a weekend-only special (Saturday and Sunday)
        var weekendSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 12 * * 6,0", // Noon on Saturday (6) and Sunday (0)
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(12, 0), // 12:00 PM
            EndTime = new LocalTime(23, 59)   // 11:59 PM
        };

        // Should be active on Saturday and Sunday
        var saturdayAfternoon = new LocalDateTime(2024, 1, 20, 15, 0); // Saturday 3 PM
        var sundayAfternoon = new LocalDateTime(2024, 1, 21, 15, 0); // Sunday 3 PM
        
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(weekendSpecial, saturdayAfternoon));
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(weekendSpecial, sundayAfternoon));
        
        // Should NOT be active on weekdays
        var mondayAfternoon = new LocalDateTime(2024, 1, 15, 15, 0); // Monday 3 PM
        var fridayAfternoon = new LocalDateTime(2024, 1, 19, 15, 0); // Friday 3 PM
        
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(weekendSpecial, mondayAfternoon));
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(weekendSpecial, fridayAfternoon));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("* * * *")] // Missing day field
    [InlineData("0 0 32 * *")] // Invalid day (32)
    [InlineData("60 * * * *")] // Invalid minute (60)
    public void IsSpecialCurrentlyActive_InvalidCronExpressions_ShouldReturnFalse(string invalidCron)
    {
        // Test that invalid CRON expressions are handled gracefully
        var specialWithInvalidCron = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = invalidCron,
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Should be false regardless of time (within or outside range)
        var withinTimeRange = new LocalDateTime(2024, 1, 15, 15, 0); // Monday 3 PM
        var outsideTimeRange = new LocalDateTime(2024, 1, 15, 8, 0); // Monday 8 AM

        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(specialWithInvalidCron, withinTimeRange));
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(specialWithInvalidCron, outsideTimeRange));
    }

    [Fact]
    public void IsSpecialCurrentlyActive_InactiveSpecial_ShouldReturnFalse()
    {
        // Test that inactive specials are never active
        var inactiveSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = false, // Inactive
            IsRecurring = true,
            CronSchedule = "0 * * * *", // Every hour
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(0, 0),  // All day
            EndTime = new LocalTime(23, 59)
        };

        var anytime = new LocalDateTime(2024, 1, 15, 15, 0);
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(inactiveSpecial, anytime));
    }

    [Fact]
    public void IsSpecialCurrentlyActive_OutsideDateRange_ShouldReturnFalse()
    {
        // Test date range enforcement
        var limitedTimeSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * *", // Every hour
            StartDate = new LocalDate(2024, 2, 1),  // Starts February 1
            EndDate = new LocalDate(2024, 2, 29),   // Ends February 29
            StartTime = new LocalTime(0, 0),
            EndTime = new LocalTime(23, 59)
        };

        // Test before start date
        var beforeStart = new LocalDateTime(2024, 1, 31, 15, 0); // January 31
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(limitedTimeSpecial, beforeStart));
        
        // Test after end date
        var afterEnd = new LocalDateTime(2024, 3, 1, 15, 0); // March 1
        Assert.False(CronScheduleEvaluator.IsSpecialCurrentlyActive(limitedTimeSpecial, afterEnd));
        
        // Test within date range
        var withinRange = new LocalDateTime(2024, 2, 15, 15, 0); // February 15
        Assert.True(CronScheduleEvaluator.IsSpecialCurrentlyActive(limitedTimeSpecial, withinRange));
    }
}
