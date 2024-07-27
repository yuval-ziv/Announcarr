namespace Announcarr.Abstractions.Contracts.Contracts;

public class CalendarContract
{
    public List<BaseCalendarItem> CalendarItems { get; set; } = [];
    public bool IsEmpty => CalendarItems.Count == 0;

    public static CalendarContract Merge(CalendarContract first, CalendarContract second)
    {
        return new CalendarContract
        {
            CalendarItems = first.CalendarItems.Concat(second.CalendarItems).ToList(),
        };
    }
}