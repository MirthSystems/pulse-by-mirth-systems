using Application.Domain.Entities;
using Application.Domain.Helpers;
using NodaTime;

namespace Application.UnitTests;

public class CronScheduleEvaluatorTests
{
    [Fact]
    public void IsSpecialCurrentlyActive_WithValidCronSchedule_ShouldRespectDayOfWeek()
    {
        // Arrange - Create a Sunday special (CRON: 0 * * * 0)
        var sundaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 0", // Every hour on Sunday
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Saturday (should be false)
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecial, saturdayDateTime);
        
        // Assert
        Assert.False(result, "Sunday special should NOT be active on Saturday");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_WithValidCronSchedule_ShouldBeActiveOnCorrectDay()
    {
        // Arrange - Create a Sunday special (CRON: 0 * * * 0)
        var sundaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 0", // Every hour on Sunday
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Sunday within time range (should be true)
        var sundayDateTime = new LocalDateTime(2024, 1, 14, 15, 0); // Sunday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecial, sundayDateTime);
        
        // Assert
        Assert.True(result, "Sunday special should be active on Sunday within time range");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_WithInvalidCronSchedule_ShouldNotFallbackToTimeOnly()
    {
        // Arrange - Create a special with invalid CRON (this triggers the bug)
        var specialWithInvalidCron = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "invalid cron", // Invalid CRON schedule
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on any day within time range
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(specialWithInvalidCron, saturdayDateTime);
        
        // Assert - This test will FAIL with current bug, showing the issue
        Assert.False(result, "Special with invalid CRON should NOT fallback to time-only check");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_TuesdaySpecial_ShouldNotBeActiveOnSaturday()
    {
        // Arrange - Create a Tuesday special (CRON: 0 * * * 2)
        var tuesdaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 2", // Every hour on Tuesday
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Saturday (should be false)
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(tuesdaySpecial, saturdayDateTime);
        
        // Assert
        Assert.False(result, "Tuesday special should NOT be active on Saturday");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_TuesdaySpecial_ShouldBeActiveOnTuesday()
    {
        // Arrange - Create a Tuesday special (CRON: 0 * * * 2)
        var tuesdaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 2", // Every hour on Tuesday
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Tuesday within time range (should be true)
        var tuesdayDateTime = new LocalDateTime(2024, 1, 16, 15, 0); // Tuesday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(tuesdaySpecial, tuesdayDateTime);
        
        // Assert
        Assert.True(result, "Tuesday special should be active on Tuesday within time range");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_WithTimeOutsideRange_ShouldBeFalse()
    {
        // Arrange - Create a Sunday special
        var sundaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 0", // Every hour on Sunday
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Sunday but outside time range (should be false)
        var sundayEarlyMorning = new LocalDateTime(2024, 1, 14, 8, 0); // Sunday, 8:00 AM (before 10:00 AM)
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecial, sundayEarlyMorning);
        
        // Assert
        Assert.False(result, "Sunday special should NOT be active on Sunday outside time range");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_NonRecurringSpecial_ShouldWorkCorrectly()
    {
        // Arrange - Create a non-recurring special
        var nonRecurringSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = false,
            CronSchedule = null,
            StartDate = new LocalDate(2024, 1, 14), // Specific Sunday
            EndDate = new LocalDate(2024, 1, 14),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on the correct date within time range (should be true)
        var correctDateTime = new LocalDateTime(2024, 1, 14, 15, 0); // Sunday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(nonRecurringSpecial, correctDateTime);
        
        // Assert
        Assert.True(result, "Non-recurring special should be active on correct date and time");
    }

    [Fact]
    public void IsSpecialCurrentlyActive_NonRecurringSpecial_WrongDate_ShouldBeFalse()
    {
        // Arrange - Create a non-recurring special for Sunday
        var nonRecurringSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = false,
            CronSchedule = null,
            StartDate = new LocalDate(2024, 1, 14), // Specific Sunday
            EndDate = new LocalDate(2024, 1, 14),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on a different date (should be false)
        var wrongDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday, 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(nonRecurringSpecial, wrongDateTime);
        
        // Assert
        Assert.False(result, "Non-recurring special should NOT be active on wrong date");
    }

    [Theory]
    [InlineData("0 * * * 0", 2024, 1, 13, false)] // Sunday special on Saturday - should be false
    [InlineData("0 * * * 0", 2024, 1, 14, true)]  // Sunday special on Sunday - should be true
    [InlineData("0 * * * 1", 2024, 1, 15, true)]  // Monday special on Monday - should be true
    [InlineData("0 * * * 2", 2024, 1, 16, true)]  // Tuesday special on Tuesday - should be true
    [InlineData("0 * * * 2", 2024, 1, 13, false)] // Tuesday special on Saturday - should be false
    [InlineData("0 * * * 3", 2024, 1, 17, true)]  // Wednesday special on Wednesday - should be true
    [InlineData("0 * * * 4", 2024, 1, 18, true)]  // Thursday special on Thursday - should be true
    [InlineData("0 * * * 5", 2024, 1, 19, true)]  // Friday special on Friday - should be true
    [InlineData("0 * * * 6", 2024, 1, 20, true)]  // Saturday special on Saturday - should be true
    public void IsSpecialCurrentlyActive_DayOfWeekTests(string cronSchedule, int year, int month, int day, bool expectedResult)
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
        var testDateTime = new LocalDateTime(year, month, day, 15, 0); // 3:00 PM
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(special, testDateTime);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void IsSpecialCurrentlyActive_ReproduceBugWithInvalidCron_ExpectedBehavior()
    {
        // This test specifically reproduces the bug described:
        // "Sunday and Tuesday specials showing as 'Active Now' on Saturday"
        
        // Arrange - Create specials that might have malformed CRON schedules
        var sundaySpecialWithBadCron = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "malformed cron for sunday", // Invalid CRON
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        var tuesdaySpecialWithBadCron = new SpecialEntity
        {
            Id = 2,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "bad tuesday cron", // Invalid CRON
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Act - Test on Saturday at 3:00 PM (within time range)
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday, 3:00 PM
        
        var sundayResult = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecialWithBadCron, saturdayDateTime);
        var tuesdayResult = CronScheduleEvaluator.IsSpecialCurrentlyActive(tuesdaySpecialWithBadCron, saturdayDateTime);
        
        // Assert - With the current bug, these will incorrectly return true
        // After fixing, they should return false
        Assert.False(sundayResult, "Sunday special with invalid CRON should NOT be active on Saturday");
        Assert.False(tuesdayResult, "Tuesday special with invalid CRON should NOT be active on Saturday");
    }
}
