using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Radarr.Integration.Contracts;

public class RadarrCalendarItem : BaseCalendarItem
{
    public string? MovieName { get; set; }
}