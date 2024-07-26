namespace Announcarr.Abstractions.Contracts.Contracts;

public abstract class NewlyMonitoredItem : ICaptionableItem
{
    public required string CalendarItemSource { get; set; }
    public required DateTimeOffset? StartedMonitoring { get; set; }
    public string? ThumbnailUrl { get; set; }
    public abstract string? GetCaption(string dateTimeFormat);
}