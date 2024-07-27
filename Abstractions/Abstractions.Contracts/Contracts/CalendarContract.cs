namespace Announcarr.Abstractions.Contracts.Contracts;

public class CalendarContract
{
    public List<BaseCalendarItem> CalendarItems { get; set; } = [];

    public static CalendarContract Merge(CalendarContract first, CalendarContract second)
    {
        return new CalendarContract
        {
            CalendarItems = first.CalendarItems.Concat(second.CalendarItems).ToList(),
        };
    }
}