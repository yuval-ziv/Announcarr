namespace Announcarr.Abstractions.Contracts.Contracts;

public class CalendarResponse
{
    public List<BaseCalendarItem> CalendarItems { get; set; } = [];

    public static CalendarResponse Merge(CalendarResponse first, CalendarResponse second)
    {
        return new CalendarResponse
        {
            CalendarItems = first.CalendarItems.Concat(second.CalendarItems).ToList(),
        };
    }
}