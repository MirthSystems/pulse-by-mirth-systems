using Cronos;
using NodaTime;
using Application.Domain.Entities;

namespace Application.Domain.Helpers;

/// <summary>
/// Utility for evaluating CRON schedules and determining if a special is currently active
/// </summary>
public static class CronScheduleEvaluator
{
    /// <summary>
    /// Determines if a special is currently active based on its schedule and time constraints
    /// </summary>
    /// <param name="special">The special entity to evaluate</param>
    /// <param name="currentDateTime">The current date and time to evaluate against</param>
    /// <returns>True if the special is currently active, false otherwise</returns>
    public static bool IsSpecialCurrentlyActive(SpecialEntity special, LocalDateTime currentDateTime)
    {
        // First check basic active flag
        if (!special.IsActive)
            return false;

        var currentDate = currentDateTime.Date;
        var currentTime = currentDateTime.TimeOfDay;

        // Check if we're within the date range
        if (currentDate < special.StartDate)
            return false;

        if (special.EndDate.HasValue && currentDate > special.EndDate.Value)
            return false;

        // For non-recurring specials, check if we're on the exact date and within time range
        if (!special.IsRecurring)
        {
            if (currentDate != special.StartDate)
                return false;

            return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
        }

        // For recurring specials, evaluate the CRON schedule
        if (!string.IsNullOrWhiteSpace(special.CronSchedule))
        {
            try
            {
                var cronExpression = CronExpression.Parse(special.CronSchedule);
                
                // Convert to UTC DateTime for Cronos library
                var todayStartUtc = currentDate.AtMidnight().InUtc().ToDateTimeUtc();
                var tomorrowStartUtc = currentDate.PlusDays(1).AtMidnight().InUtc().ToDateTimeUtc();
                
                bool isScheduleActive = false;
                
                // Check if there are any CRON occurrences that fall on today's date
                // Start searching from yesterday to ensure we catch today's occurrences
                var searchFromUtc = todayStartUtc.AddDays(-1);
                var nextOccurrence = cronExpression.GetNextOccurrence(searchFromUtc, TimeZoneInfo.Utc);
                
                // Look through upcoming occurrences to see if any fall on today
                while (nextOccurrence.HasValue && nextOccurrence.Value < tomorrowStartUtc)
                {
                    if (nextOccurrence.Value.Date == todayStartUtc.Date)
                    {
                        isScheduleActive = true;
                        break;
                    }
                    
                    // Get the next occurrence after this one
                    nextOccurrence = cronExpression.GetNextOccurrence(nextOccurrence.Value, TimeZoneInfo.Utc);
                }

                // If today doesn't match, check if yesterday had an occurrence that might still be running
                if (!isScheduleActive)
                {
                    var yesterdayStartUtc = currentDate.PlusDays(-1).AtMidnight().InUtc().ToDateTimeUtc();
                    var yesterdayOccurrence = cronExpression.GetNextOccurrence(yesterdayStartUtc.AddDays(-1), TimeZoneInfo.Utc);
                    
                    if (yesterdayOccurrence.HasValue && yesterdayOccurrence.Value.Date == yesterdayStartUtc.Date)
                    {
                        // Check if the special from yesterday extends into today
                        if (special.EndTime.HasValue && special.StartTime > special.EndTime.Value)
                        {
                            // This is a special that crosses midnight (e.g., 23:00 to 02:00)
                            isScheduleActive = currentTime <= special.EndTime.Value;
                        }
                    }
                }

                if (!isScheduleActive)
                    return false;

                // If CRON schedule is active, check time range
                return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
            }
            catch
            {
                // If CRON parsing fails, the special should NOT be active
                // Don't fall back to time-only checking as this ignores the day constraint
                return false;
            }
        }

        // If no CRON schedule for a recurring special, it should not be active
        // Recurring specials require a valid CRON schedule to determine when they occur
        return false;
    }

    /// <summary>
    /// Checks if the current time is within the specified time range
    /// </summary>
    private static bool IsWithinTimeRange(LocalTime currentTime, LocalTime startTime, LocalTime? endTime)
    {
        if (currentTime < startTime)
            return false;

        if (endTime.HasValue && currentTime > endTime.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the next occurrence of a recurring special
    /// </summary>
    public static LocalDateTime? GetNextOccurrence(SpecialEntity special, LocalDateTime fromDateTime)
    {
        if (!special.IsRecurring || string.IsNullOrWhiteSpace(special.CronSchedule))
            return null;

        try
        {
            var cronExpression = CronExpression.Parse(special.CronSchedule);
            var systemDateTime = fromDateTime.ToDateTimeUnspecified();
            
            var nextOccurrence = cronExpression.GetNextOccurrence(systemDateTime, TimeZoneInfo.Utc);
            if (!nextOccurrence.HasValue)
                return null;

            // Convert back to LocalDateTime and combine with special's start time
            var localNextDate = LocalDate.FromDateTime(nextOccurrence.Value);
            return localNextDate + special.StartTime;
        }
        catch
        {
            return null;
        }
    }
}
