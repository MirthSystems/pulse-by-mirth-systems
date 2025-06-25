namespace Application.Domain.Enums;

public enum Days
{
    Sunday = 1,
    Monday = 2,
    Tuesday = 3,
    Wednesday = 4,
    Thursday = 5,
    Friday = 6,
    Saturday = 7,
    Weekday = Monday | Tuesday | Wednesday | Thursday | Friday,
    Weekend = Saturday | Sunday
}
