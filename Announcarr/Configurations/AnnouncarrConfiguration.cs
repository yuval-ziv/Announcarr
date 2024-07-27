namespace Announcarr.Configurations;

public class AnnouncarrConfiguration
{
    public const string SectionName = "Announcarr";
    public required AnnouncarrIntervalConfiguration Interval { get; set; }
    public required AnnouncarrEmptyAnnouncementFallbackConfiguration EmptyAnnouncementFallback { get; set; }
}

public class AnnouncarrIntervalConfiguration
{
    public AnnouncerRange AnnouncerRange { get; set; }

    public int? MinuteOfHour { get; set; }
    public int? HourOfDay { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public int? MonthOfYear { get; set; }
    public string? CronAnnouncerRange { get; set; }
    public TimeSpan? CustomAnnouncerRange { get; set; }
}

public class AnnouncarrEmptyAnnouncementFallbackConfiguration
{
    public bool DoNotSendOnEmpty { get; set; } = true;
    public string CustomMessage { get; set; } = "There is nothing to announce";
}