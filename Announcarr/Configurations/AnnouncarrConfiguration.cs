namespace Announcarr.Configurations;

public class AnnouncarrConfiguration
{
    public const string SectionName = "Announcarr";
    public required AnnouncarrIntervalConfiguration Interval { get; set; }
    public required AnnouncarrEmptyContractFallbackConfiguration EmptyContractFallback { get; set; }
}

public class AnnouncarrIntervalConfiguration
{
    public AnnouncarrRange AnnouncarrRange { get; set; }

    public int? MinuteOfHour { get; set; }
    public int? HourOfDay { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public int? MonthOfYear { get; set; }
    public string? CronAnnouncarrRange { get; set; }
}

public class AnnouncarrEmptyContractFallbackConfiguration
{
    public bool ExportOnEmptyContract { get; set; } = false;
    public string CustomMessageOnEmptyContract { get; set; } = "There is nothing to announce for {announcementType}";
}