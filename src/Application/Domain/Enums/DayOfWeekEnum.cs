namespace Application.Domain.Enums;

/// <summary>
/// Represents days of the week as an enum
/// Values correspond to PostgreSQL enum and can be easily converted to/from DayOfWeekEntity
/// </summary>
public enum DayOfWeekEnum
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}
