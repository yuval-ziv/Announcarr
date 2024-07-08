using Announcer.Integrations.Abstractions.Responses;

namespace Announcer.Integrations.Sonarr.Contracts;

public class SonarrCalendarItem : BaseCalendarItem
{
    public string? SeriesName { get; set; }
    public List<Season> Seasons { get; set; } = [];
}