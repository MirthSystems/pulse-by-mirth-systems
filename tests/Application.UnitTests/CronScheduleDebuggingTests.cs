using Application.Domain.Entities;
using Application.Domain.Helpers;
using NodaTime;

namespace Application.UnitTests;

public class CronScheduleDebuggingTests
{
    [Fact]
    public void Debug_CronEvaluation_SundaySpecialOnSaturday()
    {
        // Create a Sunday special (CRON: 0 * * * 0 = every hour on Sunday)
        var sundaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 0", // Every hour on Sunday (day 0)
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Saturday, Jan 13, 2024 at 3:00 PM
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0); // Saturday
        
        // This should be FALSE but is returning TRUE due to the bug
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecial, saturdayDateTime);
        
        // Let's also test what day of week Saturday actually is
        var dayOfWeek = saturdayDateTime.Date.DayOfWeek;
        
        // Output for debugging
        System.Console.WriteLine($"Testing date: {saturdayDateTime.Date} (Day of week: {dayOfWeek})");
        System.Console.WriteLine($"Sunday special on Saturday result: {result}");
        System.Console.WriteLine($"Expected: False, Actual: {result}");
        
        // The test should fail, showing us the bug
        Assert.False(result, $"Sunday special should NOT be active on {dayOfWeek} ({saturdayDateTime.Date})");
    }

    [Fact]
    public void Debug_CronEvaluation_SundaySpecialOnSunday()
    {
        // Create the same Sunday special
        var sundaySpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "0 * * * 0", // Every hour on Sunday (day 0)
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Sunday, Jan 14, 2024 at 3:00 PM
        var sundayDateTime = new LocalDateTime(2024, 1, 14, 15, 0); // Sunday
        
        // This should be TRUE
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(sundaySpecial, sundayDateTime);
        
        var dayOfWeek = sundayDateTime.Date.DayOfWeek;
        
        // Output for debugging
        System.Console.WriteLine($"Testing date: {sundayDateTime.Date} (Day of week: {dayOfWeek})");
        System.Console.WriteLine($"Sunday special on Sunday result: {result}");
        System.Console.WriteLine($"Expected: True, Actual: {result}");
        
        Assert.True(result, $"Sunday special should be active on {dayOfWeek} ({sundayDateTime.Date})");
    }

    [Fact]
    public void Debug_CronEvaluation_InvalidCronFallback()
    {
        // Create a special with invalid CRON
        var invalidCronSpecial = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "invalid cron expression", // Invalid
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0), // 10:00 AM
            EndTime = new LocalTime(22, 0)    // 10:00 PM
        };

        // Test on Saturday at 3:00 PM (within time range)
        var saturdayDateTime = new LocalDateTime(2024, 1, 13, 15, 0);
        
        // This should be FALSE (no valid schedule) but might be TRUE due to fallback
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(invalidCronSpecial, saturdayDateTime);
        
        var dayOfWeek = saturdayDateTime.Date.DayOfWeek;
        
        System.Console.WriteLine($"Testing date: {saturdayDateTime.Date} (Day of week: {dayOfWeek})");
        System.Console.WriteLine($"Invalid CRON special result: {result}");
        System.Console.WriteLine($"Expected: False, Actual: {result}");
        
        // This test shows the fallback bug
        Assert.False(result, $"Invalid CRON special should NOT be active on any day");
    }
}
