namespace Announcarr.Abstractions.Contracts;

public abstract class BaseCalendarItem : ICaptionableItem, IThumbnailItem
{
    public required string CalendarItemSource { get; set; }
    public required DateTimeOffset? ReleaseDate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public abstract string? GetCaption(string dateTimeFormat);
    public string? GetThumbnailUri() => ThumbnailUrl;
}