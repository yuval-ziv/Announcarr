namespace Announcarr.Abstractions.Contracts;

public class CalendarContract : BaseAnnouncement
{
    public List<BaseCalendarItem> CalendarItems { get; set; } = [];
    public override bool IsEmpty => CalendarItems.Count == 0;
    public override AnnouncementType AnnouncementType => AnnouncementType.Calendar;

    public static CalendarContract Merge(CalendarContract first, CalendarContract second)
    {
        return new CalendarContract
        {
            CalendarItems = first.CalendarItems.Concat(second.CalendarItems).ToList(),
            Tags = first.Tags.Concat(second.Tags).ToHashSet(),
        };
    }
}