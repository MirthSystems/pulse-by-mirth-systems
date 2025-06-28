using Application.Domain.Entities;
using Application.Domain.Helpers;
using NodaTime;

namespace Application.UnitTests;

public class CronEmptyStringDebugTests
{
    [Fact]
    public void TestEmptyStringBehavior()
    {
        var emptyString = "";
        var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(emptyString);
        
        Console.WriteLine($"Empty string: '{emptyString}'");
        Console.WriteLine($"IsNullOrWhiteSpace: {isNullOrWhiteSpace}");
        
        // This should be true
        Assert.True(isNullOrWhiteSpace, "Empty string should be considered null or whitespace");
    }

    [Fact]
    public void Debug_EmptyStringCronSchedule()
    {
        var specialWithEmptyString = new SpecialEntity
        {
            Id = 1,
            IsActive = true,
            IsRecurring = true,
            CronSchedule = "", // Empty string
            StartDate = new LocalDate(2024, 1, 1),
            EndDate = new LocalDate(2024, 12, 31),
            StartTime = new LocalTime(10, 0),
            EndTime = new LocalTime(22, 0)
        };

        var testDateTime = new LocalDateTime(2024, 1, 15, 15, 0);
        var result = CronScheduleEvaluator.IsSpecialCurrentlyActive(specialWithEmptyString, testDateTime);
        
        Console.WriteLine($"CronSchedule: '{specialWithEmptyString.CronSchedule}'");
        Console.WriteLine($"IsNullOrWhiteSpace: {string.IsNullOrWhiteSpace(specialWithEmptyString.CronSchedule)}");
        Console.WriteLine($"Result: {result}");
        
        // Since it's an empty string, it should fall through to time-only check
        // But we want to verify which code path it's taking
        Assert.False(result, "Empty CRON schedule should not be active");
    }
}
