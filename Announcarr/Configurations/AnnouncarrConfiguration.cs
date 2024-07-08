namespace Announcarr.Configurations;

public class AnnouncarrConfiguration
{
    public const string SectionName = "Announcarr";

    public AnnouncerRange AnnouncerRange { get; set; }

    public int? MinuteOfHour { get; set; }
    public int? HourOfDay { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public int? MonthOfYear { get; set; }
    public string? CronAnnouncerRange { get; set; }
    public TimeSpan? CustomAnnouncerRange { get; set; }
}