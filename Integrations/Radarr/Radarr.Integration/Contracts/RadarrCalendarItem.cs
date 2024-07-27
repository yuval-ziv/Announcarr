using Announcarr.Abstractions.Contracts;
using EnumsNET;

namespace Announcarr.Integrations.Radarr.Integration.Contracts;

public class RadarrCalendarItem : BaseCalendarItem
{
    public string? MovieName { get; set; }
    public ReleaseDateType ReleaseDateType { get; set; }

    public override string? GetCaption(string dateTimeFormat)
    {
        return $"{MovieName}{(ReleaseDate is not null ? $" - {ReleaseDateType.AsString(EnumFormat.Description)} on {ReleaseDate?.ToString(dateTimeFormat)}" : string.Empty)}";
    }
}