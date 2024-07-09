using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Sonarr.Integration.Contracts;

public class SonarrCalendarItem : BaseCalendarItem
{
    public string? SeriesName { get; set; }
    public List<Season> Seasons { get; set; } = [];
}