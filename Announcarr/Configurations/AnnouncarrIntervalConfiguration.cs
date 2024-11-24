namespace Announcarr.Configurations;

public class AnnouncarrIntervalConfiguration
{
    public AnnouncarrRange AnnouncarrRange { get; set; }

    public int? MinuteOfHour { get; set; }
    public int? HourOfDay { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public int? MonthOfYear { get; set; }
}