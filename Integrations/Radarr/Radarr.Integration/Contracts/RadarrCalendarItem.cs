using System.ComponentModel;
using Announcarr.Integrations.Abstractions.Responses;
using EnumsNET;

namespace Announcarr.Integrations.Radarr.Integration.Contracts;

public class RadarrCalendarItem : BaseCalendarItem
{
    public string? MovieName { get; set; }
    public ReleaseDateType ReleaseDateType { get; set; }

    public override string? GetCaption(string dateTimeFormat) =>
        $"{MovieName}{(ReleaseDate is not null ? $" - {ReleaseDateType.AsString(EnumFormat.Description)} on {ReleaseDate?.ToString(dateTimeFormat)}" : string.Empty)}";
}

public enum ReleaseDateType
{
    [Description("physical release")] PhysicalRelease,
    [Description("digital release")] DigitalRelease,
    [Description("in cinemas")] InCinemas,
}