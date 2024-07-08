namespace Announcer.Integrations.Abstractions.Responses;

public abstract class BaseCalendarItem
{
    public required string CalendarItemSource { get; set; }
    public string? ThumbnailUrl { get; set; }
}