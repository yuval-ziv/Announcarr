using Announcer.Integrations.Abstractions.Responses;

namespace Announcer.Integrations.Radarr.Contracts;

public class RadarrCalendarItem : BaseCalendarItem
{
    public string? MovieName { get; set; }
}